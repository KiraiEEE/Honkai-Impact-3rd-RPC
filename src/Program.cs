using DiscordRPC;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HonkaiImpactRpc;

internal static class Program
{
    private const string AppId = "1540429775439397034";
    private const string GameClassName = "UnityWndClass";
    private const string GameWindowName = "Honkai Impact 3rd";
    private static readonly TimeSpan MinUpdateInterval = TimeSpan.FromSeconds(15);

    private static DiscordRpcClient _client;
    private static NotifyIcon _notifyIcon;

    [STAThread]
    static void Main()
    {
        using var self = new Mutex(true, "Honkai Impact 3rd DiscordRPC", out var allow);
        if (!allow)
        {
            MessageBox.Show("Honkai Impact 3rd DiscordRPC is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _client = new DiscordRpcClient(AppId);
            _client.Initialize();

            var playing = false;
            var lastUpdateTime = DateTime.MinValue;

            while (true)
            {
                await Task.Delay(1000);

                var handle = FindWindow(GameClassName, GameWindowName);

                if (handle == IntPtr.Zero || !IsWindow(handle))
                {
                    if (playing)
                    {
                        playing = false;
                        _client.ClearPresence();
                        Debug.Print("Game window lost, cleared presence");
                    }
                    continue;
                }

                if (playing)
                    continue;

                var now = DateTime.UtcNow;
                if (now - lastUpdateTime < MinUpdateInterval)
                    continue;

                try
                {
                    GetWindowThreadProcessId(handle, out var pid);
                    using var process = Process.GetProcessById(pid);
                    Debug.Print($"Game found: {process.ProcessName} (PID {pid})");

                    playing = true;
                    lastUpdateTime = DateTime.UtcNow;
                    _client.SetPresence(new RichPresence
                    {
                        Assets = new Assets
                        {
                            LargeImageKey = "logo",
                            LargeImageText = "Honkai Impact 3rd",
                        },
                        Timestamps = Timestamps.Now,
                    });
                    Debug.Print("RichPresence set");
                }
                catch (Exception e)
                {
                    Debug.Print($"Error: {e.Message}{Environment.NewLine}{e.StackTrace}");
                }
            }
        });

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var exitButton = new ToolStripMenuItem("Exit");
        var autoButton = new ToolStripMenuItem("AutoStart");
        autoButton.Checked = AutoStart.Check();

        var strip = new ContextMenuStrip();
        strip.Items.Add(autoButton);
        strip.Items.Add(new ToolStripSeparator());
        strip.Items.Add(exitButton);

        _notifyIcon = new NotifyIcon()
        {
            BalloonTipIcon = ToolTipIcon.Info,
            ContextMenuStrip = strip,
            Text = "Honkai Impact 3rd DiscordRPC",
            Icon = Properties.Resources.tray,
            Visible = true,
        };

        exitButton.Click += (_, _) =>
        {
            _client?.ClearPresence();
            _notifyIcon.Visible = false;
            Thread.Sleep(100);
            Environment.Exit(0);
        };
        autoButton.Click += (_, _) =>
        {
            autoButton.Checked = !autoButton.Checked;

            if (autoButton.Checked)
                AutoStart.Set();
            else
                AutoStart.Remove();
        };

        Application.Run();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
}
