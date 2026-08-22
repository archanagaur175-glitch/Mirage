using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Mirage.Core;
using Mirage.Core.Services;
using Mirage.Native;
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

        // Topmost, tool-window, layered (so it floats above the shell).
        int ex = WindowStyles.GetExStyle(_hwnd);
        WindowStyles.SetExStyle(_hwnd, ex | NativeConstants.WS_EX_TOPMOST | NativeConstants.WS_EX_TOOLWINDOW | NativeConstants.WS_EX_LAYERED);

        // Tahoe-style Mica backdrop.
        Dwm.SetBackdropType(_hwnd, NativeConstants.DWMSBT_MAINWINDOW);
        Dwm.SetCornerPreference(_hwnd, 2 /* DWMWCP_ROUND */);

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
            Panel.Children.Add(item);
        }
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
