using DiscordRPC;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarRailDiscordRpc;

internal static class Program
{
    private const string Impact = "1113930060513153096";
    private const string StarRail = "1100789242029948999";

    [STAThread]
    static void Main()
    {
        using var self = new Mutex(true, "Honkai DiscordRPC", out var allow);
        if (!allow)
        {
            MessageBox.Show("Honkai DiscordRPC is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(-1);
        }

        if (Properties.Settings.Default.IsFirstTime)
        {
            AutoStart.Set();
            Properties.Settings.Default.IsFirstTime = false;
            Properties.Settings.Default.Save();
        }

        Task.Run(async () =>
        {
            using var clientZh = new DiscordRpcClient(Impact);
            using var clientEn = new DiscordRpcClient(StarRail);
            clientZh.Initialize();
            clientEn.Initialize();

            var playing = false;

            while (true)
            {
                await Task.Delay(1000);

                Debug.Print($"InLoop");

                var miHoyo = true;
                var handle = FindWindow("UnityWndClass", "Honkai Impact 3rd");
                if (handle == IntPtr.Zero)
                {
                    // hoyoverse
                    miHoyo = false;
                    handle = FindWindow("UnityWndClass", "Honkai: Star Rail");
                }

                if (handle == IntPtr.Zero)
                {
                    Debug.Print($"Not found game process.");
                    playing = false;
                    if (clientEn.CurrentPresence != null)
                    {
                        clientEn.ClearPresence();
                    }
                    if (clientZh.CurrentPresence != null)
                    {
                        clientZh.ClearPresence();
                    }
                    continue;
                }

                try
                {
                    var process = Process.GetProcesses().First(x => x.MainWindowHandle == handle);

                    Debug.Print($"Check process with {handle} | {process.ProcessName}");

                    if (miHoyo)
                    {
                        if (!playing)
                        {
                            playing = true;
                            clientZh.UpdateRpc("iconx", "Honkai Impact 3rd");
                            Debug.Print($"Set RichPresence to {process.ProcessName}");
                        }
                        else
                        {
                            Debug.Print($"Keep RichPresence to {process.ProcessName}");
                        }
                    }
                    else
                    {
                        if (!playing)
                        {
                            playing = true;
                            clientEn.UpdateRpc("logo", "Honkai: Star Rail");
                            Debug.Print($"Set RichPresence to  {process.ProcessName}");
                        }
                        else
                        {
                            Debug.Print($"Keep RichPresence to {process.ProcessName}");
                        }
                    }
                }
                catch (Exception e)
                {
                    playing = false;
                    if (clientEn.CurrentPresence != null)
                    {
                        clientEn.ClearPresence();
                    }
                    if (clientZh.CurrentPresence != null)
                    {
                        clientZh.ClearPresence();
                    }
                    Debug.Print($"{e.Message}{Environment.NewLine}{e.StackTrace}");
                }

                GC.Collect();
                GC.WaitForFullGCComplete();
            }
        });

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var notifyMenu = new ContextMenu();
        var exitButton = new MenuItem("Exit");
        var autoButton = new MenuItem("AutoStart" + "    " + (AutoStart.Check() ? "√" : "✘"));
        notifyMenu.MenuItems.Add(0, autoButton);
        notifyMenu.MenuItems.Add(1, exitButton);

        var notifyIcon = new NotifyIcon()
        {
            BalloonTipIcon = ToolTipIcon.Info,
            ContextMenu = notifyMenu,
            Text = "Honkai DiscordRPC",
            Icon = Properties.Resources.tray,
            Visible = true,
        };

        exitButton.Click += (_, _) =>
        {
            notifyIcon.Visible = false;
            Thread.Sleep(100);
            Environment.Exit(0);
        };
        autoButton.Click += (_, _) =>
        {
            if (AutoStart.Check())
            {
                AutoStart.Remove();
            }
            else
            {
                AutoStart.Set();
            }

            autoButton.Text = "AutoStart" + "    " + (AutoStart.Check() ? "√" : "✘");
        };


        Application.Run();
    }

    private static void UpdateRpc(this DiscordRpcClient client, string key, string text)
        => client.SetPresence(new RichPresence
        {
            Assets = new Assets
            {
                LargeImageKey = key,
                LargeImageText = text,
            },
            Timestamps = Timestamps.Now,
        });

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
}
