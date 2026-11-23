using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace LootPulse;

public static class GuiWindow
{
    private static Form? _form;
    private static WebView2? _webView;
    private static NotifyIcon? _trayIcon;

    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    public static void Launch(string url, string dataDir, Action onExit)
    {
        var thread = new Thread(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();

            _form = new Form
            {
                Text = "LootPulse",
                Width = 1140,
                Height = 780,
                MinimumSize = new Size(720, 520),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.FromArgb(12, 14, 18)
            };

            // Set Form Icon from embedded asset or Exe
            try
            {
                var iconStream = typeof(GuiWindow).Assembly.GetManifestResourceStream("app_icon.ico");
                if (iconStream != null) _form.Icon = new Icon(iconStream);
            }
            catch { /* fallback */ }

            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            _form.Controls.Add(_webView);

            // Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = _form.Icon,
                Text = "LootPulse - Fortnite Tracker",
                Visible = true
            };

            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Открыть LootPulse", null, (_, _) => RestoreWindow());
            trayMenu.Items.Add("Выход", null, (_, _) =>
            {
                _trayIcon.Visible = false;
                onExit();
                Application.Exit();
            });
            _trayIcon.ContextMenuStrip = trayMenu;
            _trayIcon.DoubleClick += (_, _) => RestoreWindow();

            _form.FormClosing += (_, e) =>
            {
                _trayIcon.Visible = false;
                onExit();
            };

            InitWebView(url, Path.Combine(dataDir, "webview"));

            Application.Run(_form);
        })
        {
            IsBackground = false,
            Name = "LootPulse.GuiWindow"
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    private static async void InitWebView(string url, string cacheFolder)
    {
        try
        {
            var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, cacheFolder);
            if (_webView != null)
            {
                await _webView.EnsureCoreWebView2Async(env);
                _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

                _webView.CoreWebView2.WebMessageReceived += (_, e) =>
                {
                    try
                    {
                        var msg = e.TryGetWebMessageAsString();
                        if (msg == "wdrag")
                        {
                            if (_form != null && !_form.IsDisposed)
                            {
                                ReleaseCapture();
                                SendMessage(_form.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                            }
                        }
                        else if (msg is "wmin" or "wmax" or "wclose")
                        {
                            HandleWindowControl(msg);
                        }
                    }
                    catch { /* message guard */ }
                };

                _webView.Source = new Uri(url);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"WebView2 Runtime initialization error: {ex.Message}", "LootPulse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static void HandleWindowControl(string act)
    {
        if (_form == null || _form.IsDisposed) return;

        _form.BeginInvoke(new Action(() =>
        {
            switch (act)
            {
                case "wdrag":
                    ReleaseCapture();
                    SendMessage(_form.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                    break;
                case "wmin":
                    _form.WindowState = FormWindowState.Minimized;
                    break;
                case "wmax":
                    _form.WindowState = _form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                    break;
                case "wclose":
                    _form.Close();
                    break;
            }
        }));
    }

    public static void RestoreWindow()
    {
        if (_form == null || _form.IsDisposed) return;

        _form.BeginInvoke(new Action(() =>
        {
            if (_form.WindowState == FormWindowState.Minimized)
            {
                _form.WindowState = FormWindowState.Normal;
            }
            _form.Show();
            _form.BringToFront();
        }));
    }
}
