using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JustDance201X
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

        int[] imageindex = { 2016, 2017, 2018, 2019, 2020 };
        public string culture = System.Globalization.CultureInfo.InstalledUICulture.NativeName;
        public string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public MainWindow()
        {
            InitializeComponent();
           
            
            if (culture.Contains("中国") ==false)
            {
                button.Content = "START";
                textBlock_JD.Width = 200;
                textBlock_JD.Text = "JUSTDANCE";
                textBlock_PCS.Width = 300;
                textBlock_PCS.Margin = new Thickness(188,0,0,0);
                textBlock_PCS.Text = "SpecialEdition";
                textBlock_SELVR.Text = "Choose a version to play with";
                textBlock_MANZE.Text = "Disclaimer: this starter program and mobile app are free to download. They are strictly prohibited for commercial use. The developer shall not bear any legal responsibility.";
                textBlock_DEVINFO.Text = "Developer email: encvstin@qq.com                [Mobile app built based on Dolphindroid]";
            }

            if (File.Exists(@Environment.CurrentDirectory + "\\Dolphin.exe"))
            {
                int existrom = 0;
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2016.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2017.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2018.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2019.wbfs")) { existrom += 1; }
                if (File.Exists(@Environment.CurrentDirectory + "\\ROM\\JD2020.wbfs")) { existrom += 1; }
                if (existrom == 5)
                { }
                else
                {
                    if (culture.Contains("中国"))
                    {
                        MessageBox.Show("游戏固件不完整或不存在，请将舞力全开2016-2020的固件以JD20XX.wbfs命名存放在ROM文件夹下");
                    }
                    else
                    {
                        MessageBox.Show("The game firmware is incomplete or does not exist. Please store the firmware of JUSTDANCE 2016-2020 in the ROM folder named jd20xx.wbfs");
                    }
                    Application.Current.Shutdown();
                }
                if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini"))
                {

                }
                else
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = $"{ @Environment.CurrentDirectory}\\Dolphin.exe",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = false
                        });
                    }
                    catch
                    {
                        Application.Current.Shutdown();
                    }
                    if (culture.Contains("中国"))
                    {
                        MessageBox.Show("没有找到Dolphin 3.5 模拟器配置文件，请先关闭模拟器再重新打开本程序");

                    }
                    else
                    {
                        MessageBox.Show("Dolphin simulator 3.5 configuration file was not found. Please close the simulator and open the program again");
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
                    MessageBox.Show("Dolphin simulator 3.5 was not found. please keep this program in the same directory as dolphin");
                }
                Application.Current.Shutdown();
            }

            imagex_2.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[0] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_1.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[1] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[2] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_3.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[3] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_4.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[4] + ".jpg", UriKind.RelativeOrAbsolute));
			if (culture.Contains("中国"))
            {      
				textBlock_HELP.Text = "使用安卓手机扫码下载安装控制器，打开后输入电脑IP [" + GetLocalIP() + "] 及连接码即可进行游玩";
			}
			else{textBlock_HELP.Text = "Use Android mobile phone to scan code, install and open controller, input IP [ " + GetLocalIP() + " ] and player connect code to play";
          ;
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
                else {
                    textBlock_P4.Text = "----";
                }
            }

        }


        private void imagex_3_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_3.Opacity += 0.1;
        }

        private void imagex_3_MouseLeave(object sender, MouseEventArgs e)
        {

            imagex_3.Opacity -= 0.1;
        }

        private void imagex_4_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_4.Opacity += 0.1;
        }

        private void imagex_4_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_4.Opacity -= 0.1;
        }

        private void imagex_1_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_1.Opacity += 0.1;
        }

        private void imagex_1_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_1.Opacity -= 0.1;
        }
        private void imagex_2_MouseEnter(object sender, MouseEventArgs e)
        {
            imagex_2.Opacity += 0.1;
        }


        private void imagex_2_MouseLeave(object sender, MouseEventArgs e)
        {
            imagex_2.Opacity -= 0.1;
        }

        private void imagex_3_MouseDown(object sender, MouseButtonEventArgs e)
        {

            reindex(imageindex, 5, 1, 1);

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
            imagex_2.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[0] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_1.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[1] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[2] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_3.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[3] + ".jpg", UriKind.RelativeOrAbsolute));
            imagex_4.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageindex[4] + ".jpg", UriKind.RelativeOrAbsolute));

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





        private void imagex_4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 5, 2, 1);
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void imagex_1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 5, 1, 0);
        }

        private void imagex_2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            reindex(imageindex, 5, 2, 0);
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

        private void image1_Copy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Random rd = new Random();
            int iRandom = rd.Next(500, 600);
            if (File.Exists(@path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini"))
            {
                INIWriter("General", "GCMPath0", @Environment.CurrentDirectory+"\\ROM", @path + "\\Documents\\Dolphin Emulator\\Config\\Dolphin.ini");
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
                INIWriter("Wiimote1", "UDP Wiimote/Port", iRandom+"1", @path + "\\Documents\\Dolphin Emulator\\Config\\WiimoteNew.ini");

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



        private void button_Click(object sender, RoutedEventArgs e)
        {
            string args = $"--exec {@Environment.CurrentDirectory}\\ROM\\JD{imageindex[2]}.wbfs";
            //MessageBox.Show(args);
            try
            {
                Process.Start(new ProcessStartInfo
                {

                    FileName = $"{ @Environment.CurrentDirectory}\\Dolphin.exe",
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

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
