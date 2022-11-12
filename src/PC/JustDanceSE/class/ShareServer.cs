using System.IO;
using System.Net;
using System.Web;
using System.Threading;
using System;

namespace Share{ 
        public class DownloadCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 发出下载请求的地址和端口号
    /// </summary>
    public IPEndPoint RemoteEndPoint;

    /// <summary>
    /// HTTP状态代码
    /// </summary>
    public HttpStatusCode StatusCode;

    /// <summary>
    /// 如果StatusCode为HttpStatusCode.Forbidden，则为非法请求的RawUrl，否则为异常消息
    /// </summary>
    public string Message;

    /// <param name="endpoint">网络端点</param>
    /// <param name="code">状态代码</param>
    /// <param name="message">异常消息或者非法请求的RawUrl</param>
    public DownloadCompletedEventArgs(IPEndPoint endpoint, HttpStatusCode code, string message = null)
    {
        RemoteEndPoint = endpoint;
        StatusCode = code;
        Message = message;
    }
    }

        /// <summary>
        /// 支持单一文件下载的HTTP服务器类
        /// </summary>
        public class ShareServer : IDisposable
        {
            /// <summary>
            /// 下载事件处理器委托定义
            /// </summary>
            /// <param name="sender">事件发送者</param>
            /// <param name="e">事件参数</param>
            public delegate void DownloadCompletedEventHandler(object sender, DownloadCompletedEventArgs e);

            /// <summary>
            /// 下载事件处理器
            /// </summary>
            public event DownloadCompletedEventHandler OnDownloadCompletedEvent;

            /// <summary>
            /// HTTP协议侦听器
            /// </summary>
            private HttpListener Listener;

            /// <summary>
            /// 服务器访问链接
            /// </summary>
            public string ServerUrl { get; private set; }

            /// <summary>
            /// 下载文件访问链接
            /// </summary>
            public string FileUrl { get; private set; }

            /// <summary>
            /// 文件原始名称
            /// </summary>
            public string FileRawName { get; set; }

            /// <summary>
            /// 文件编码名称
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// 文件名称，前面需要带“/”
            /// </summary>
            private string FileRawUrl;

            /// <summary>
            /// 文件内容
            /// </summary>
            private byte[] FileContent;

            /// <summary>
            /// 开启HTTP协议侦听器
            /// </summary>
            /// <param name="path">文件路径</param>
            /// <param name="ip">侦听地址，可以设为“*”和“+”</param>
            /// <param name="port">侦听端口，默认为9920</param>
            /// <returns>
            ///     true：成功
            ///     false：失败
            /// </returns>
            public bool Start(string path, string ip, int port = 9920)
            {
                try
                {
                    Dispose();
                    if (!File.Exists(path)) return false;

                    // 获取文件名称
                    FileRawName = new FileInfo(path).Name; // 文件原始名称
                    FileName = System.Web.HttpUtility.UrlEncode(FileRawName); // 文件编码名称
                    FileRawUrl = "/" + FileName;

                    // 读取文件内容
                    using (FileStream fs = File.OpenRead(path))
                    {
                        FileContent = new byte[fs.Length];
                        fs.Read(FileContent, 0, (int)fs.Length);
                    }

                    // 启动HTTP侦听
                    Listener = new HttpListener
                    {
                        AuthenticationSchemes = AuthenticationSchemes.Anonymous // 客户端身份验证方案
                    };

                    ServerUrl = string.Format("http://{0}:{1}/", ip, port); // 服务器访问链接
                    Listener.Prefixes.Add(ServerUrl);
                    FileUrl = ServerUrl + FileName; // 文件访问链接                
                    AddUrlAcl(ServerUrl); // 增加Url访问控制列表（管理员权限）

                    // 启动侦听
                    Listener.Start();
                    new Thread(new ThreadStart(ListenThreadAction)).Start();
                    return true;
                }
                catch
                {
                    Dispose();
                    return false;
                }
            }

            /// <summary>
            /// 侦听连接线程
            /// </summary>
            private void ListenThreadAction()
            {
                while (true)
                {
                    try
                    {
                        HttpListenerContext context = Listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;
                        if (string.Equals(request.RawUrl, FileRawUrl, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
                            {
                                try
                                {
                                    response.StatusCode = (int)HttpStatusCode.OK;
                                    response.ContentType = "application/octet-stream"; // 二进制流数据
                                    response.AddHeader("Content-Disposition", "attachment;filename=" + FileName); // 直接进入文件下载操作
                                    response.AddHeader("Access-Control-Allow-Origin", "*");
                                    using (Stream output = response.OutputStream)
                                    {
                                        int Length = FileContent.Length;
                                        response.ContentLength64 = Length;
                                        output.Write(FileContent, 0, Length);
                                    }

                                    if (OnDownloadCompletedEvent != null)
                                    {
                                        OnDownloadCompletedEvent(this, new DownloadCompletedEventArgs(request.RemoteEndPoint, HttpStatusCode.OK));
                                    }
                                }
                                catch (Exception exception)
                                {
                                    if (OnDownloadCompletedEvent != null)
                                    {
                                        //{ OnDownloadCompletedEvent(this, new DownloadCompletedEventArgs(request.RemoteEndPoint, HttpStatusCode.InternalServerError, exception.Message)); } 
                                    }
                                }
                            }));
                        }
                        else
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
                            {
                                try
                                {
                                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                                    response.ContentType = "text/plain";
                                    response.ContentEncoding = System.Text.Encoding.UTF8;
                                    using (Stream output = response.OutputStream)
                                    {
                                        byte[] Buffer = System.Text.Encoding.UTF8.GetBytes("Forbidden");
                                        int Length = Buffer.Length;
                                        response.ContentLength64 = Length;
                                        output.Write(Buffer, 0, Length);
                                    }

                                    if (OnDownloadCompletedEvent != null)
                                    {
                                        OnDownloadCompletedEvent(this, new DownloadCompletedEventArgs(request.RemoteEndPoint, HttpStatusCode.Forbidden, request.RawUrl));
                                    }
                                }
                                catch (Exception exception)
                                {
                                    if (OnDownloadCompletedEvent != null)
                                    {
                                        OnDownloadCompletedEvent(this, new DownloadCompletedEventArgs(request.RemoteEndPoint, HttpStatusCode.InternalServerError, exception.Message));
                                    }
                                }
                            }));
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// 删除Url访问控制列表
            /// </summary>
            /// <param name="url">链接地址</param>
            /// <returns>命令执行结果信息</returns>
            /// <remarks>要求管理员权限</remarks>
            public static string DeleteUrlAcl(string url)
            {
                return CmdExecute(string.Format(@"netsh http delete urlacl url={0}", url));
            }

            /// <summary>
            /// 增加Url访问控制列表
            /// </summary>
            /// <param name="url">链接地址</param>
            /// <returns>命令执行结果信息</returns>
            /// <remarks>要求管理员权限</remarks>
            public static string AddUrlAcl(string url)
            {
                return CmdExecute(string.Format(@"netsh http add urlacl url={0} user={1}\{2}", url, Environment.UserDomainName, Environment.UserName));
            }

            /// <summary>
            /// 在命令窗口执行命令并获取输出结果
            /// </summary>
            /// <param name="command">要执行的命令</param>
            /// <param name="workingDirectory">工作目录</param>
            /// <param name="milliseconds">超时等待毫秒数</param>
            /// <returns>命令执行结果</returns>
            public static string CmdExecute(string command, string workingDirectory = null, int milliseconds = -1)
            {
                // 设置进程启动信息
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("CMD.exe", "/C " + command);

                if (!string.IsNullOrEmpty(workingDirectory)) startInfo.WorkingDirectory = workingDirectory; // 工作目录
                startInfo.UseShellExecute = false; // 可以重定向 IO 流
                startInfo.RedirectStandardInput = true; // 重定向标准输入流
                startInfo.RedirectStandardOutput = true; // 重定向标准输出流
                startInfo.RedirectStandardError = true; // 重定向标准错误流
                startInfo.CreateNoWindow = true; // 隐藏窗体

                // 创建进程执行命令
                using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(startInfo))
                {
                    string output = proc.StandardOutput.ReadToEnd(); // 从标准输出流中获取命令执行结果
                    proc.WaitForExit(milliseconds);
                    return output;
                }
            }

            /// <summary>
            /// 侦听服务是否已启动
            /// </summary>
            /// <returns>
            ///     true：已启动
            ///     false：未启动
            /// </returns>
            public bool IsListening { get { return Listener.IsListening; } }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (Listener != null)
                {
                    if (Listener.IsListening) Listener.Stop();
                    Listener.Close();
                    Listener = null;
                }

                if (!string.IsNullOrEmpty(ServerUrl))
                {   // 删除Url访问控制列表
                    DeleteUrlAcl(ServerUrl);
                    ServerUrl = null;
                }
            }
        }
    }

