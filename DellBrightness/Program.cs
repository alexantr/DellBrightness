using DellBrightness.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;

namespace DellBrightness
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainWindow());
            Application.Run(new AppContext());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }

    public class AppContext : ApplicationContext
    {
        private readonly string ddmPath = "C:\\Program Files (x86)\\Dell\\Dell Display Manager\\ddm.exe";

        public AppContext()
        {
            NotifyIcon trayIcon = new NotifyIcon
            {
                Icon = Resources.Brightness,
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            if (File.Exists(ddmPath))
            {
                trayIcon.ContextMenu.MenuItems.AddRange(new MenuItem[] {
                    new MenuItem("0%", (s, e) => { SetBrightness("0"); }),
                    new MenuItem("5%", (s, e) => { SetBrightness("5"); }),
                    new MenuItem("10%", (s, e) => { SetBrightness("10"); }),
                    new MenuItem("15%", (s, e) => { SetBrightness("15"); }),
                    new MenuItem("20%", (s, e) => { SetBrightness("20"); }),
                    new MenuItem("25%", (s, e) => { SetBrightness("25"); }),
                    new MenuItem("30%", (s, e) => { SetBrightness("30"); }),
                    new MenuItem("40%", (s, e) => { SetBrightness("40"); }),
                    new MenuItem("50%", (s, e) => { SetBrightness("50"); }),
                    new MenuItem("100%", (s, e) => { SetBrightness("100"); })
                });
            }
            else
            {
                trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Dell Display Manager not found!") { Enabled = false });

                trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Download Dell Display Manager", (s, e) => {
                    Process.Start("http://www.dell.com/ddm");
                }));
            }

            trayIcon.ContextMenu.MenuItems.Add("-");
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Run at Startup", (s, e) =>
            {
                bool isRun = isRunAtStartup();
                SetStartup(!isRun);
                (s as MenuItem).Checked = !isRun;
            }) { Checked = isRunAtStartup() });

            trayIcon.ContextMenu.MenuItems.Add("-");
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", (s, e) => {
                trayIcon.Visible = false;
                Application.Exit();
            }));
        }

        private void SetStartup(bool RunAtStartup)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (RunAtStartup)
                rk.SetValue(Application.ProductName, Application.ExecutablePath);
            else
                rk.DeleteValue(Application.ProductName, false);
        }

        private bool isRunAtStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            var val = rk.GetValue(Application.ProductName);

            return val + "" == Application.ExecutablePath;
        }

        private void SetBrightness(string brightness)
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ddmPath,
                    Arguments = "/SetBrightnessLevel " + brightness,
                    //CreateNoWindow = true,
                    //UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
        }
    }
}
