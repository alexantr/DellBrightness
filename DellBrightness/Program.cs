using DellBrightness.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        private readonly string ddm2Path = "C:\\Program Files\\Dell\\Dell Display Manager 2\\DDM.exe";

        public AppContext()
        {
            NotifyIcon trayIcon = new NotifyIcon
            {
                Icon = Resources.Brightness,
                Visible = true,
                ContextMenu = new ContextMenu()
            };

            if (File.Exists(ddmPath) || File.Exists(ddm2Path))
            {
                MenuItem more = new MenuItem("More...");
                more.MenuItems.AddRange(new MenuItem[] {
                    new MenuItem("55%", (s, e) => { SetBrightness("55"); }),
                    new MenuItem("60%", (s, e) => { SetBrightness("60"); }),
                    new MenuItem("65%", (s, e) => { SetBrightness("65"); }),
                    new MenuItem("70%", (s, e) => { SetBrightness("70"); }),
                    new MenuItem("75%", (s, e) => { SetBrightness("75"); }),
                    new MenuItem("80%", (s, e) => { SetBrightness("80"); }),
                    new MenuItem("85%", (s, e) => { SetBrightness("85"); }),
                    new MenuItem("90%", (s, e) => { SetBrightness("90"); }),
                    new MenuItem("95%", (s, e) => { SetBrightness("95"); }),
                    new MenuItem("100%", (s, e) => { SetBrightness("100"); }),
                    more
                });

                trayIcon.ContextMenu.MenuItems.AddRange(new MenuItem[] {
                    new MenuItem("0%", (s, e) => { SetBrightness("0"); }),
                    new MenuItem("2%", (s, e) => { SetBrightness("2"); }),
                    new MenuItem("5%", (s, e) => { SetBrightness("5"); }),
                    new MenuItem("10%", (s, e) => { SetBrightness("10"); }),
                    new MenuItem("15%", (s, e) => { SetBrightness("15"); }),
                    new MenuItem("20%", (s, e) => { SetBrightness("20"); }),
                    new MenuItem("25%", (s, e) => { SetBrightness("25"); }),
                    new MenuItem("30%", (s, e) => { SetBrightness("30"); }),
                    new MenuItem("35%", (s, e) => { SetBrightness("35"); }),
                    new MenuItem("40%", (s, e) => { SetBrightness("40"); }),
                    new MenuItem("45%", (s, e) => { SetBrightness("45"); }),
                    new MenuItem("50%", (s, e) => { SetBrightness("50"); }),
                    more
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
            string fileName, arguments;
            if (File.Exists(ddm2Path))
            {
                fileName = ddm2Path;
                arguments = "/WriteBrightnessLevel " + brightness;
            }
            else if (File.Exists(ddmPath))
            {
                fileName = ddmPath;
                arguments = "/SetBrightnessLevel " + brightness;
            }
            else
            {
                return;
            }

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    //CreateNoWindow = true,
                    //UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
        }
    }
}
