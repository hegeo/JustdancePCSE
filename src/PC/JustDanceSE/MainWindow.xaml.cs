using Share;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ThoughtWorks.QRCode.Codec;

namespace JustDanceSE
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
        private ShareServer Server;

        int[] imageindex = { 2022, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021 };
        public string culture = System.Globalization.CultureInfo.InstalledUICulture.NativeName;
        public string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public MainWindow()
        {
            InitializeComponent();
            if (culture.Contains("中国") == false)
            {
                button.Content = "Dance Now!";
                textBlock_JD.Width = 200;
                textBlock_JD.Text = "JUSTDANCE";
                textBlock_PCS.Width = 300;
                textBlock_PCS.Margin = new Thickness(188, 0, 0, 0);
                textBlock_PCS.Text = "SpecialEdition2";
                textBlock_VERS.Visibility = Visibility.Collapsed;
                textBlock_SELVR.Text = "Choose a version to play with";
                textBlock_ROMSTAT.Text = "ROM was not found";
                textBlock_MANZE.Text = "Disclaimer: this starter program and mobile app are free to download. They are strictly prohibited for commercial use. The developer shall not bear any legal responsibility.";
                textBlock_DEVINFO.Text = "hegeo@foxmail.com";

            }


            Directory.CreateDirectory(@Environment.CurrentDirectory + "\\Station\\");
            if (File.Exists(@Environment.CurrentDirectory + "\\Station\\app.apk") == false)
            {
                ExtractFile("JustDanceSE.addons.jdsecapp.apk", @Environment.CurrentDirectory + "\\Station\\app.apk");
            }

            if (File.Exists(@Environment.CurrentDirectory + "\\Station\\Dolphin.exe") == false)
            {
                ExtractFile("JustDanceSE.addons.dolphin.exe", @Environment.CurrentDirectory + "\\Station\\upgrade.exe");
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{@Environment.CurrentDirectory}\\Station\\upgrade.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false
                });
                System.Threading.Thread.Sleep(5000);
            }

            if (File.Exists(@Environment.CurrentDirectory + "\\Station\\Dolphin.exe"))
            {
                if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini") == false)
                {
                    Directory.CreateDirectory(@path + "\\Documents\\Dolphin Emulator\\Config\\");
                    ExtractFile("JustDanceSE.addons.Dolphin.ini", path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                }

                if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini") == false)
                {
                    Directory.CreateDirectory(@path + "\\Documents\\Dolphin Emulator\\Config\\");
                    ExtractFile("JustDanceSE.addons.gfx_opengl.ini", path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                }

                Directory.CreateDirectory(@Environment.CurrentDirectory + "\\ROM\\");
                int existrom = 0;
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2016.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2017.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2018.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2019.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2020.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2021.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2022.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2014.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2015.wbfs")) { existrom += 1; }

                if (existrom > 0)
                {
                    if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini"))
                    {

                    }
                    else
                    {
                        if (File.Exists(@Environment.CurrentDirectory + "\\Station\\Dolphin.exe"))
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = $"{@Environment.CurrentDirectory}\\Station\\Dolphin.exe",
                                    UseShellExecute = false,
                                    CreateNoWindow = true,
                                    RedirectStandardOutput = false
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }
                        if (culture.Contains("中国"))
                        {
                            MessageBox.Show("Dolphin 模拟器未初始化，请先关闭模拟器，再重新打开本启动器");
                        }
                        else
                        {
                            MessageBox.Show("Dolphin emulator is not initialized. Please turn off the emulator first, and then turn on the initiator again");
                        }
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();


                    }
                }

                else
                {
                    if (culture.Contains("中国"))
                    {
                        MessageBox.Show("没有找到任何游戏固件，请将舞力全开的游戏固件以JD20XX.wbfs命名存放在ROM文件夹下");
                        System.Diagnostics.Process.Start("https://bing.com/search?q=justdance+wii+wbfs");
                    }
                    else
                    {
                        MessageBox.Show("No game firmware found. Please download the game firmware of JUSTDANCE in the ROM folder named jd20xx.wbfs");
                        System.Diagnostics.Process.Start("https://bing.com/search?q=dolphin%20emulator%203.5");
                    }
                    Application.Current.Shutdown();
                }
            }
            else
            {
                if (culture.Contains("中国"))
                {
                    MessageBox.Show("未找到Dolphin 3.5 模拟器或相关组件，请保持本程序在Dolphin.exe同目录下");
                }
                else
                {
                    MessageBox.Show("Dolphin emulator 3.5 was not found. please keep this program in the same directory as dolphin");
                }
                Application.Current.Shutdown();
            }


            if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD" + imageindex[0] + ".wbfs")) { textBlock_ROMSTAT.Visibility = Visibility.Collapsed; button.Visibility = Visibility.Visible; }
            else { textBlock_ROMSTAT.Visibility = Visibility.Visible; button.Visibility = Visibility.Collapsed; }

            ImageBrush b1 = new ImageBrush();
            b1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[0] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_1.Background = b1;
            ImageBrush b2 = new ImageBrush();
            b2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[1] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_2.Background = b2;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[2] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_3.Background = b3;
            ImageBrush b4 = new ImageBrush();
            b4.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[3] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_4.Background = b4;
            ImageBrush b5 = new ImageBrush();
            b5.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[4] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_5.Background = b5;
            ImageBrush b6 = new ImageBrush();
            b6.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[5] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_6.Background = b6;
            ImageBrush b7 = new ImageBrush();
            b7.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[6] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_7.Background = b7;
            ImageBrush b8 = new ImageBrush();
            b8.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[7] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_8.Background = b8;
            ImageBrush b9 = new ImageBrush();
            b9.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[8] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_9.Background = b9;

            if (culture.Contains("中国"))
            {
                textBlock_HELP.Text = "使用安卓手机扫码下载安装控制器，打开后输入电脑IP [" + GetLocalIP() + "] 及连接码即可进行游玩，按下[空格键]退出游戏";
            }
            else
            {
                textBlock_HELP.Text = "Use Android phone to scan barcode, install and open controller, input IP [ " + GetLocalIP() + " ] and connect code to play，press [SPACE] exit game";
                ;
            }
            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini") == false)
            {
                Directory.CreateDirectory(@path + "\\Documents\\Dolphin Emulator\\Config\\");
                ExtractFile("JustDanceSE.addons.WiimoteNew.ini", path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
            }

            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini"))
            {
                if (INIReader("Wiimote1", "UDP Wiimote/Enable", "0", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini") == "1")
                {
                    textBlock_P1.Text = INIReader("Wiimote1", "UDP Wiimote/Port ", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                }
                else
                {
                    textBlock_P1.Text = "----";
                }
                if (INIReader("Wiimote2", "UDP Wiimote/Enable", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini") == "1")
                {
                    textBlock_P2.Text = INIReader("Wiimote2", "UDP Wiimote/Port ", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                }
                else
                {
                    textBlock_P2.Text = "----";
                }
                if (INIReader("Wiimote3", "UDP Wiimote/Enable", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini") == "1")
                {
                    textBlock_P3.Text = INIReader("Wiimote3", "UDP Wiimote/Port ", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                }
                else
                {
                    textBlock_P3.Text = "----";
                }
                if (INIReader("Wiimote4", "UDP Wiimote/Enable", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini") == "1")
                {
                    textBlock_P4.Text = INIReader("Wiimote4", "UDP Wiimote/Port ", "*", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                }
                else
                {
                    textBlock_P4.Text = "----";
                }
            }

        }

        private void OnDownloadCompletedEvent(object sender, DownloadCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                if (culture.Contains("中国"))
                {
                    MessageBox.Show("您的手机已经下载客户端！");
                }
                else { MessageBox.Show("Your mobile phone has downloaded the client!"); }

            }));

        }

        private void ExtractFile(String resource, String path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            BufferedStream input = new BufferedStream(assembly.GetManifestResourceStream(resource));
            FileStream output = new FileStream(path, FileMode.Create);
            byte[] data = new byte[1024];
            int lengthEachRead;
            while ((lengthEachRead = input.Read(data, 0, data.Length)) > 0)
            {
                output.Write(data, 0, lengthEachRead);
            }
            output.Flush();
            output.Close();
        }

        public void reindex(int[] arr, int n, int p, int lr)
        {
            if (lr == 1)
            {
                reverse(arr, 0, p - 1);
                reverse(arr, p, n - 1);
                reverse(arr, 0, n - 1);
            }
            else if (lr == 0)
            {
                reverse(arr, n - p, n - 1);
                reverse(arr, 0, n - p - 1);
                reverse(arr, 0, n - 1);
            }

            if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD" + imageindex[0] + ".wbfs")) { textBlock_ROMSTAT.Visibility = Visibility.Collapsed; button.Visibility = Visibility.Visible; }
            else { textBlock_ROMSTAT.Visibility = Visibility.Visible; button.Visibility = Visibility.Collapsed; }

            ImageBrush b1 = new ImageBrush();
            b1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[0] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_1.Background = b1;
            ImageBrush b2 = new ImageBrush();
            b2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[1] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_2.Background = b2;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[2] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_3.Background = b3;
            ImageBrush b4 = new ImageBrush();
            b4.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[3] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_4.Background = b4;
            ImageBrush b5 = new ImageBrush();
            b5.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[4] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_5.Background = b5;
            ImageBrush b6 = new ImageBrush();
            b6.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[5] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_6.Background = b6;
            ImageBrush b7 = new ImageBrush();
            b7.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[6] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_7.Background = b7;
            ImageBrush b8 = new ImageBrush();
            b8.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[7] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_8.Background = b8;
            ImageBrush b9 = new ImageBrush();
            b9.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[8] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_9.Background = b9;
        }

        private void reverse(int[] arr, int begin, int end)
        {
            int i, j;
            for (i = begin, j = end; i < j; i++, j--)
            {
                int t;
                t = arr[i];
                arr[i] = arr[j];
                arr[j] = t;
            }
        }


        private void exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Server.Dispose();
            Application.Current.Shutdown();
        }



        public BitmapSource GetImageSouce(Bitmap bitmap)
        {
            BitmapSource img;
            IntPtr hBitmap;

            hBitmap = bitmap.GetHbitmap();
            img = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return img;
        }

        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName();
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "NetworkError:" + ex.Message.ToString();
            }
        }

        public string INIReader(string section, string key, string def, string file)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, sb, 1024, file);
            return sb.ToString();
        }
        public void INIWriter(string section, string key, string value, string file)
        {
            WritePrivateProfileString(section, key, value, file);
        }

        private void refresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Server.Dispose();
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    if (p.ProcessName == "Dolphin")
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }
            Random rd = new Random();
            int iRandom = rd.Next(500, 600);
            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini"))
            {
                INIWriter("General", "GCMPath0", @Environment.CurrentDirectory + "\\ROM", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Display", "Fullscreen", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Display", "RenderToMain", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Hotkeys", "Stop", "32", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Hotkeys", "Exit ", "27", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Interface", "ConfirmStop", "False", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
                INIWriter("Interface", "OnScreenDisplayMessages", "False", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");

            }
            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini"))
            {

                INIWriter("Hardware", "VSync", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Settings", "EFBScale ", "0", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Settings", "MSAA", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Hacks", "EFBAccessEnable", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Hacks", "EFBCopyEnable", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Hacks", "EFBScaledCopy", "True", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");
                INIWriter("Hacks", "Enhancements", "FXAA", @path + "\\Documents\\Dolphin Emulator\\Config\\gfx_opengl.ini");




            }
            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini"))
            {
                INIWriter("Wiimote1", "Source", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote1", "UDP Wiimote/Enable", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote1", "UDP Wiimote/Port", iRandom + "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");

                INIWriter("Wiimote2", "Source", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote2", "UDP Wiimote/Enable", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote2", "UDP Wiimote/Port", iRandom + "2", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");

                INIWriter("Wiimote3", "Source", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote3", "UDP Wiimote/Enable", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote3", "UDP Wiimote/Port", iRandom + "3", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");

                INIWriter("Wiimote4", "Source", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote4", "UDP Wiimote/Enable", "1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");
                INIWriter("Wiimote4", "UDP Wiimote/Port", iRandom + "4", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");

            }
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }



        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private void imagex_2_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_2.Opacity = 1;

        }

        private void imagex_2_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_2.Opacity = 0.5;

        }

        private void imagex_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 1, 1);
        }

        private void imagex_9_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_9.Opacity = 1;

        }

        private void imagex_9_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_9.Opacity = 0.5;

        }

        private void imagex_9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 1, 0);
        }

        private void imagex_3_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_3.Opacity = 1;
        }

        private void imagex_3_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_3.Opacity = 0.5;

        }

        private void imagex_3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 2, 1);
        }

        private void imagex_8_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_8.Opacity = 1;

        }

        private void imagex_8_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_8.Opacity = 0.5;

        }

        private void imagex_8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 2, 0);
        }

        private void imagex_4_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_4.Opacity = 1;

        }

        private void imagex_4_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_4.Opacity = 0.5;

        }

        private void imagex_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 3, 1);
        }

        private void imagex_7_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_7.Opacity = 1;

        }

        private void imagex_7_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_7.Opacity = 0.5;
        }

        private void imagex_7_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 3, 0);
        }

        private void imagex_5_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_5.Opacity = 1;
        }

        private void imagex_5_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_5.Opacity = 0.5;


        }

        private void imagex_5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 4, 1);
        }

        private void imagex_6_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_6.Opacity = 1;
        }

        private void imagex_6_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_6.Opacity = 0.5;
        }

        private void imagex_6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 9, 4, 0);
        }

        private void imagex_1_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_1.Opacity = 1;
        }

        private void imagex_1_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_1.Opacity = 0.9;

        }

        private void button_MouseEnter(object sender, MouseEventArgs e)
        {
            button.Opacity = 1;
        }

        private void button_MouseLeave(object sender, MouseEventArgs e)
        {
            button.Opacity = 0.8;
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (File.Exists(@Environment.CurrentDirectory + "\\Station\\app.apk"))
            {
                try
                {   // 启动文件共享服务
                    Server = new ShareServer();
                    Server.OnDownloadCompletedEvent += OnDownloadCompletedEvent;
                    Server.Start(@Environment.CurrentDirectory + "\\Station\\app.apk", GetLocalIP(), 9922);

                    //生成二维码
                    Bitmap qrcodebt;
                    string enCodeString = Server.FileUrl;
                    //二维码参数
                    QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                    qrCodeEncoder.QRCodeScale = 2;
                    qrCodeEncoder.QRCodeVersion = 4;
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    //生成二维码图片
                    qrcodebt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
                    ImageBrush qr = new ImageBrush();
                    qr.ImageSource = GetImageSouce(qrcodebt);
                    qrcode.Background = qr;


                    //MessageBox.Show(Server.FileUrl);

                }
                catch (Exception exception)
                {
                    Server.Dispose();
                    if (culture.Contains("中国"))
                    {
                        MessageBox.Show("共享App文件失败，请确认是否为管理员模式运行！");
                    }
                    else { MessageBox.Show("Failed to share App file, please confirm whether it is running in administrator mode!"); }
                }



            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string args = $"--exec {@Environment.CurrentDirectory}\\ROM\\JD{imageindex[0]}.wbfs";
            try
            {
                Process.Start(new ProcessStartInfo
                {

                    FileName = $"{@Environment.CurrentDirectory}\\Station\\Dolphin.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = args,
                    RedirectStandardOutput = true
                });
            }

            catch
            {
                Application.Current.Shutdown();
            }
            Application.Current.Shutdown();
        }

        private void textBlock_DEVINFO_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
