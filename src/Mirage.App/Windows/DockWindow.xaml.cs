using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Mirage.Core;
using Mirage.Core.Services;
using Mirage.Native;
using Windows.UI;
using WinRT.Interop;

namespace Mirage.App;

public sealed partial class DockWindow : Window
{
    private static DockWindow? _instance;
    private readonly DockService _dock = new();
    private IntPtr _hwnd;

    public DockWindow()
    {
        this.InitializeComponent();
        _hwnd = WindowNative.GetWindowHandle(this);

        // Topmost + tool-window so it floats above the shell. NOTE: WS_EX_LAYERED is
        // intentionally NOT set — WinUI 3 does not composite XAML correctly with that
        // flag, which previously rendered the dock as a blank white bar.
        int ex = WindowStyles.GetExStyle(_hwnd);
        WindowStyles.SetExStyle(_hwnd, ex | NativeConstants.WS_EX_TOPMOST | NativeConstants.WS_EX_TOOLWINDOW);

        // Frosted Tahoe glass via the WinUI 3 acrylic backdrop (no DWM flag needed).
        this.SystemBackdrop = new DesktopAcrylicBackdrop();

        PositionAtBottom();
        Populate();
    }

    private void PositionAtBottom()
    {
        int screenW = SystemParameters.GetSystemMetrics(NativeConstants.SM_CXSCREEN);
        int screenH = SystemParameters.GetSystemMetrics(NativeConstants.SM_CYSCREEN);
        int height = 86;

        var rc = new RECT
        {
            left = 0,
            top = screenH - height,
            right = screenW,
            bottom = screenH,
        };

        _dock.Register(_hwnd, NativeConstants.ABE_BOTTOM, rc, 0);
        _dock.SetPosition(_hwnd, NativeConstants.ABE_BOTTOM, rc);

        // Keep the window glued to that reserved rect.
        WindowStyles.Position(_hwnd, NativeConstants.HWND_TOPMOST, 0, screenH - height, screenW, height,
            NativeConstants.SWP_NOACTIVATE);
    }

    private void Populate()
    {
        Panel.Children.Clear();
        foreach (var app in _dock.EnumerateRunningApps())
        {
            var item = new Controls.DockItemControl { App = app };
            item.SetRunning(true);
            item.SetGlyph(GlyphFor(app.Title), ColorFor(app.Title));
            item.Activated += OnItemActivated;
            Panel.Children.Add(item);
        }
    }

    private static void OnItemActivated(Mirage.Core.Models.RunningApp? app)
    {
        if (app is not null)
        {
            Hittest.ActivateWindow(app.Handle);
        }
    }

    private static string GlyphFor(string title)
    {
        foreach (var c in title)
        {
            if (char.IsLetterOrDigit(c))
            {
                return c.ToString().ToUpperInvariant();
            }
        }
        return "?";
    }

    private static Windows.UI.Color ColorFor(string title)
    {
        int hash = 0;
        foreach (var c in title)
        {
            hash = (hash * 31 + c) & 0x7fffffff;
        }
        return Windows.UI.Color.FromArgb(255, (byte)(50 + (hash % 160)), (byte)(90 + ((hash >> 3) % 120)), (byte)(160 + ((hash >> 6) % 80)));
    }

    public static void Show()
    {
        if (_instance is null)
        {
            _instance = new DockWindow();
        }

        _instance.Activate();
    }

    public static void CloseInstance()
    {
        if (_instance is not null)
        {
            new DockService().Remove(_instance._hwnd);
            _instance.Close();
            _instance = null;
        }
    }
}
