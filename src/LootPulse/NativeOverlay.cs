using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace LootPulse;

public static class NativeOverlay
{
    private static Thread? _thread;
    private static IntPtr _hwnd;
    private static Config _config = new();
    private static MatchEngine? _engine;
    private static PartyHub? _partyHub;

    private static volatile bool _visible = true;
    private static volatile bool _locked = false;
    private static volatile bool _gameIsForeground = true;
    private static int _x = 24, _y = 140;
    private static int _width = 410, _height = 68;

    private const int WM_TIMER = 0x0113;
    private const int WM_HOTKEY = 0x0312;
    private const int WM_PAINT = 0x000F;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_MOUSEMOVE = 0x0200;
    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_APP_UPDATE = 0x8001;

    private const int HOTKEY_TOGGLE = 1;
    private const int HOTKEY_LOCK = 2;

    public static void Start(Config config, MatchEngine engine, PartyHub partyHub)
    {
        _config = config;
        _engine = engine;
        _partyHub = partyHub;
        _x = config.OverlayX;
        _y = config.OverlayY;
        _locked = config.OverlayLocked;
        _visible = config.OverlayEnabled;

        if (_thread != null) return;

        _thread = new Thread(RunWindowLoop)
        {
            IsBackground = true,
            Name = "LootPulse.NativeOverlay"
        };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
    }

    public static void NotifyUpdate()
    {
        if (_hwnd != IntPtr.Zero)
        {
            PostMessage(_hwnd, WM_APP_UPDATE, IntPtr.Zero, IntPtr.Zero);
        }
    }

    public static void ToggleVisibility()
    {
        _visible = !_visible;
        _config.OverlayEnabled = _visible;
        UpdateVisibility();
        NotifyUpdate();
    }

    public static void SetOnlyInGame(bool onlyInGame)
    {
        _config.OverlayOnlyInGame = onlyInGame;
        if (_hwnd != IntPtr.Zero)
        {
            PostMessage(_hwnd, WM_TIMER, IntPtr.Zero, IntPtr.Zero);
        }
    }

    private static void UpdateVisibility()
    {
        if (_hwnd != IntPtr.Zero)
        {
            bool show = _visible && (!_config.OverlayOnlyInGame || _gameIsForeground);
            ShowWindow(_hwnd, show ? 8 : 0); // SW_SHOWNA = 8, SW_HIDE = 0
        }
    }

    private static void CheckGameForeground()
    {
        if (!_config.OverlayOnlyInGame)
        {
            if (!_gameIsForeground)
            {
                _gameIsForeground = true;
                UpdateVisibility();
            }
            return;
        }

        var fg = GetForegroundWindow();
        bool isGame = false;
        if (fg == _hwnd)
        {
            isGame = _gameIsForeground;
        }
        else if (fg != IntPtr.Zero)
        {
            GetWindowThreadProcessId(fg, out var pid);
            if (pid != 0)
            {
                try
                {
                    using var proc = System.Diagnostics.Process.GetProcessById((int)pid);
                    if (proc.ProcessName.StartsWith("Fortnite", StringComparison.OrdinalIgnoreCase))
                    {
                        isGame = true;
                    }
                }
                catch
                {
                    // Process exited or inaccessible
                }
            }
        }

        if (isGame != _gameIsForeground)
        {
            _gameIsForeground = isGame;
            UpdateVisibility();
        }
    }

    public static void ToggleLock()
    {
        _locked = !_locked;
        _config.OverlayLocked = _locked;
        if (_hwnd != IntPtr.Zero)
        {
            UpdateWindowExStyle();
            NotifyUpdate();
        }
    }

    public static void SetOpacity(int percent)
    {
        _config.OverlayOpacity = Math.Clamp(percent, 0, 100);
        NotifyUpdate();
    }

    public static void SetTextOpacity(int percent)
    {
        _config.OverlayTextOpacity = Math.Clamp(percent, 10, 100);
        NotifyUpdate();
    }

    private static void RunWindowLoop()
    {
        var wndClass = new WNDCLASSEX
        {
            cbSize = Marshal.SizeOf<WNDCLASSEX>(),
            style = 0x0002 | 0x0020, // CS_HREDRAW | CS_OWNDC
            lpfnWndProc = WndProc,
            hInstance = GetModuleHandle(null),
            lpszClassName = "LootPulse_OverlayClass",
            hCursor = LoadCursor(IntPtr.Zero, 32512) // IDC_ARROW
        };
        RegisterClassEx(ref wndClass);

        var exStyle = 0x00080000 | 0x00000008 | 0x08000000; // WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_NOACTIVATE
        if (_locked) exStyle |= 0x00000020; // WS_EX_TRANSPARENT (click-through)

        _hwnd = CreateWindowEx(
            exStyle,
            "LootPulse_OverlayClass",
            "LootPulse HUD",
            0x80000000 | 0x10000000, // WS_POPUP | WS_VISIBLE
            _x, _y, _width, _height,
            IntPtr.Zero, IntPtr.Zero, GetModuleHandle(null), IntPtr.Zero);

        RenderOverlay(_hwnd);

        // Register Global Hotkeys: Ctrl+Shift+O (Toggle), Ctrl+Shift+L (Lock)
        RegisterHotKey(_hwnd, HOTKEY_TOGGLE, 0x0002 | 0x0004, 0x4F); // MOD_CONTROL | MOD_SHIFT + 'O'
        RegisterHotKey(_hwnd, HOTKEY_LOCK, 0x0002 | 0x0004, 0x4C);   // MOD_CONTROL | MOD_SHIFT + 'L'

        SetTimer(_hwnd, (UIntPtr)1, 250, IntPtr.Zero);
        CheckGameForeground();
        UpdateVisibility();

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0))
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);
        }

        KillTimer(_hwnd, (UIntPtr)1);
        UnregisterHotKey(_hwnd, HOTKEY_TOGGLE);
        UnregisterHotKey(_hwnd, HOTKEY_LOCK);
    }

    private static void UpdateWindowExStyle()
    {
        var exStyle = 0x00080000 | 0x00000008 | 0x08000000;
        if (_locked) exStyle |= 0x00000020;
        SetWindowLong(_hwnd, -20, exStyle); // GWL_EXSTYLE = -20
    }

    private static bool _isDragging;
    private static Point _dragStart;

    private static IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_TIMER:
                CheckGameForeground();
                return IntPtr.Zero;

            case WM_HOTKEY:
                if (wParam.ToInt32() == HOTKEY_TOGGLE) ToggleVisibility();
                else if (wParam.ToInt32() == HOTKEY_LOCK) ToggleLock();
                return IntPtr.Zero;

            case WM_LBUTTONDOWN:
                if (!_locked)
                {
                    _isDragging = true;
                    GetCursorPos(out var p);
                    _dragStart = new Point(p.X - _x, p.Y - _y);
                    SetCapture(hWnd);
                }
                return IntPtr.Zero;

            case WM_MOUSEMOVE:
                if (_isDragging)
                {
                    GetCursorPos(out var p);
                    _x = p.X - _dragStart.X;
                    _y = p.Y - _dragStart.Y;
                    _config.OverlayX = _x;
                    _config.OverlayY = _y;
                    SetWindowPos(hWnd, IntPtr.Zero, _x, _y, _width, _height, 0x0004 | 0x0010); // SWP_NOZORDER | SWP_NOACTIVATE
                }
                return IntPtr.Zero;

            case WM_LBUTTONUP:
                if (_isDragging)
                {
                    _isDragging = false;
                    ReleaseCapture();
                }
                return IntPtr.Zero;

            case WM_APP_UPDATE:
                RenderOverlay(hWnd);
                return IntPtr.Zero;

            case WM_PAINT:
                BeginPaint(hWnd, out var ps);
                EndPaint(hWnd, ref ps);
                return IntPtr.Zero;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private static void RenderOverlay(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero || !_visible || (_config.OverlayOnlyInGame && !_gameIsForeground)) return;

        var peers = _partyHub?.GetConnectedPeers() ?? (IReadOnlyList<TeammatePeer>)Array.Empty<TeammatePeer>();
        int targetHeight = 64 + (peers.Count > 0 ? (peers.Count * 22 + 6) : 0);
        _height = targetHeight;

        using var bmp = new Bitmap(_width, _height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.Clear(Color.Transparent);

            // Background card (HUD GUI) - opacity controlled by OverlayOpacity (0-100%)
            int bgAlpha = (int)Math.Round(Math.Clamp(_config.OverlayOpacity, 0, 100) * 2.55);
            if (bgAlpha > 0)
            {
                using var bgBrush = new SolidBrush(Color.FromArgb(bgAlpha, 10, 12, 16));
                using var borderPen = new Pen(Color.FromArgb((int)(bgAlpha * 0.7), 229, 169, 60), 1.2f);
                var rect = new Rectangle(0, 0, _width - 1, _height - 1);
                FillRoundedRectangle(g, bgBrush, rect, 8);
                DrawRoundedRectangle(g, borderPen, rect, 8);
            }

            // HUD Text - opacity controlled independently by OverlayTextOpacity (10-100%)
            int textAlpha = (int)Math.Round(Math.Clamp(_config.OverlayTextOpacity, 10, 100) * 2.55);
            using var fontHeader = new Font("Segoe UI", 9, FontStyle.Bold);
            using var fontBold = new Font("Segoe UI", 10, FontStyle.Bold);
            using var fontRow = new Font("Segoe UI", 10, FontStyle.Regular);

            using var whiteBrush = new SolidBrush(Color.FromArgb(textAlpha, 245, 245, 247));
            using var goldBrush = new SolidBrush(Color.FromArgb(textAlpha, 229, 169, 60));
            using var muteBrush = new SolidBrush(Color.FromArgb((int)(textAlpha * 0.65), 150, 155, 170));
            using var divPen = new Pen(Color.FromArgb((int)(Math.Max(bgAlpha, textAlpha) * 0.25), 255, 255, 255), 1f);

            // Columns layout: Player, Last, All, Session
            const int colPlayer = 16;
            const int colLast = 180;
            const int colAll = 255;
            const int colSession = 335;

            // Header Row
            g.DrawString("Player", fontHeader, muteBrush, colPlayer, 10);
            g.DrawString("Last", fontHeader, muteBrush, colLast, 10);
            g.DrawString("All", fontHeader, muteBrush, colAll, 10);
            g.DrawString("Session", fontHeader, muteBrush, colSession, 10);

            g.DrawLine(divPen, 16, 28, _width - 16, 28);

            // Row 1: Local Player
            var myName = !string.IsNullOrWhiteSpace(_config.Profile) ? _config.Profile : Environment.MachineName;
            int myLast = _engine?.LastMatchChests ?? 0;
            int myAll = _engine?.LifetimeChests ?? 0;
            int mySession = _engine?.SessionChests ?? 0;

            int curY = 34;
            g.DrawString(myName, fontBold, goldBrush, colPlayer, curY);
            g.DrawString(myLast.ToString(), fontBold, goldBrush, colLast, curY);
            g.DrawString(myAll.ToString("N0"), fontBold, whiteBrush, colAll, curY);
            g.DrawString(mySession.ToString(), fontBold, goldBrush, colSession, curY);

            // Connected Teammates Rows
            if (peers.Count > 0)
            {
                curY += 24;
                g.DrawLine(divPen, 16, curY, _width - 16, curY);
                curY += 4;

                foreach (var p in peers)
                {
                    int pLast = p.LastMatchChests > 0 ? p.LastMatchChests : p.MatchChests;
                    g.DrawString(p.PlayerName, fontRow, whiteBrush, colPlayer, curY);
                    g.DrawString(pLast.ToString(), fontRow, goldBrush, colLast, curY);
                    g.DrawString(p.TotalChests > 0 ? p.TotalChests.ToString("N0") : "-", fontRow, whiteBrush, colAll, curY);
                    g.DrawString(p.SessionChests.ToString(), fontRow, goldBrush, colSession, curY);
                    curY += 22;
                }
            }
        }

        IntPtr screenDc = GetDC(IntPtr.Zero);
        IntPtr memDc = CreateCompatibleDC(screenDc);

        var bmi = new BITMAPINFO
        {
            biSize = Marshal.SizeOf<BITMAPINFO>(),
            biWidth = _width,
            biHeight = -_height,
            biPlanes = 1,
            biBitCount = 32,
            biCompression = 0
        };

        IntPtr hBitmap = CreateDIBSection(screenDc, ref bmi, 0, out IntPtr pBits, IntPtr.Zero, 0);
        IntPtr oldBmp = SelectObject(memDc, hBitmap);

        var bmpData = bmp.LockBits(new Rectangle(0, 0, _width, _height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        CopyMemory(pBits, bmpData.Scan0, (uint)(_height * bmpData.Stride));
        bmp.UnlockBits(bmpData);

        var blend = new BLENDFUNCTION
        {
            BlendOp = 0,
            BlendFlags = 0,
            SourceConstantAlpha = 255,
            AlphaFormat = 1 // AC_SRC_ALPHA
        };
        var pos = new POINT { X = _x, Y = _y };
        var size = new SIZE { cx = _width, cy = _height };
        var zero = new POINT { X = 0, Y = 0 };

        UpdateLayeredWindow(hWnd, screenDc, ref pos, ref size, memDc, ref zero, 0, ref blend, 2);

        SelectObject(memDc, oldBmp);
        DeleteObject(hBitmap);
        DeleteDC(memDc);
        ReleaseDC(IntPtr.Zero, screenDc);
    }

    private static void FillRoundedRectangle(Graphics g, Brush brush, Rectangle r, int d)
    {
        using var path = CreateRoundedRectPath(r, d);
        g.FillPath(brush, path);
    }

    private static void DrawRoundedRectangle(Graphics g, Pen pen, Rectangle r, int d)
    {
        using var path = CreateRoundedRectPath(r, d);
        g.DrawPath(pen, path);
    }

    private static GraphicsPath CreateRoundedRectPath(Rectangle r, int d)
    {
        var path = new GraphicsPath();
        path.AddArc(r.X, r.Y, d, d, 180, 90);
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    #region Win32 P/Invoke
    [StructLayout(LayoutKind.Sequential)]
    private struct WNDCLASSEX
    {
        public int cbSize, style;
        public WndProcDelegate lpfnWndProc;
        public int cbClsExtra, cbWndExtra;
        public IntPtr hInstance, hIcon, hCursor, hbrBackground;
        public string? lpszMenuName, lpszClassName;
        public IntPtr hIconSm;
    }

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X, Y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct SIZE { public int cx, cy; }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFO
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public Rectangle rcPaint;
        public bool fRestore, fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
    }

    [DllImport("user32.dll")] private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);
    [DllImport("user32.dll")] private static extern IntPtr CreateWindowEx(int dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
    [DllImport("user32.dll")] private static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);
    [DllImport("user32.dll")] private static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport("user32.dll")] private static extern bool TranslateMessage(ref MSG lpMsg);
    [DllImport("user32.dll")] private static extern IntPtr DispatchMessage(ref MSG lpMsg);
    [DllImport("user32.dll")] private static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
    [DllImport("user32.dll")] private static extern IntPtr BeginPaint(IntPtr hWnd, out PAINTSTRUCT lpPaint);
    [DllImport("user32.dll")] private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);
    [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
    [DllImport("user32.dll")] private static extern IntPtr SetCapture(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern bool ReleaseCapture();
    [DllImport("user32.dll")] private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
    [DllImport("user32.dll")] private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
    [DllImport("user32.dll")] private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)] private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, uint crKey, ref BLENDFUNCTION pblend, uint dwFlags);
    [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
    [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hDC);
    [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);
    [DllImport("gdi32.dll")] private static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")] private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
    [DllImport("kernel32.dll")] private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    private struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam, lParam; public uint time; public POINT pt; }
    #endregion
}
