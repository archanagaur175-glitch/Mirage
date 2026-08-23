using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Mirage.Native;
using Windows.Graphics;
using Windows.UI;
using WinRT.Interop;

namespace Mirage.App;

/// <summary>
/// Out-of-process overlay window drawn over a third-party app's title bar. It
/// occludes the native caption buttons and forwards our macOS-style close /
/// min / max clicks to the target via PostMessage(WM_SYSCOMMAND). The target
/// process is never injected into, subclassed, or modified.
/// </summary>
public sealed class TrafficLightOverlayWindow : Window
{
    private IntPtr _target;
    private readonly IntPtr _self;

    public TrafficLightOverlayWindow(IntPtr target)
    {
        _target = target;
        Title = "Mirage Traffic Lights";

        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(8, 0, 0, 0),
        };

        panel.Children.Add(MakeLight(this, "#FFFF5F57", NativeConstants.SC_CLOSE));
        panel.Children.Add(MakeLight(this, "#FFFEBC2E", NativeConstants.SC_MINIMIZE));
        panel.Children.Add(MakeLight(this, "#FF28C840", NativeConstants.SC_MAXIMIZE));

        Content = panel;

        AppWindow?.Resize(new SizeInt32(66, 26));

        _self = WindowNative.GetWindowHandle(this);

        // Topmost + tool-window only (no WS_EX_LAYERED — WinUI 3 renders blank
        // with that flag). The overlay is a small dark pill carrying the lights.
        int ex = WindowStyles.GetExStyle(_self);
        WindowStyles.SetExStyle(_self, ex | NativeConstants.WS_EX_TOPMOST | NativeConstants.WS_EX_TOOLWINDOW);

        this.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(220, 28, 28, 32));

        Reposition();
        Activate();
    }

    public IntPtr Target => _target;
    public IntPtr Handle => _self;

    /// <summary>Point the overlay at a different window and re-anchor it.</summary>
    public void SetTarget(IntPtr newTarget)
    {
        _target = newTarget;
        Reposition();
    }

    private static Button MakeLight(TrafficLightOverlayWindow owner, string color, int command)
    {
        var button = new Button
        {
            Width = 12,
            Height = 12,
            CornerRadius = new CornerRadius(6),
            Background = new SolidColorBrush(ColorFromString(color)),
        };
        button.Click += (s, e) => Hittest.PostSysCommand(owner.Target, command);
        return button;
    }

    private static Windows.UI.Color ColorFromString(string hex)
    {
        // Accepts #AARRGGBB or #RRGGBB.
        var c = hex.TrimStart('#');
        if (c.Length == 8)
        {
            byte a = System.Convert.ToByte(c.Substring(0, 2), 16);
            byte r = System.Convert.ToByte(c.Substring(2, 2), 16);
            byte g = System.Convert.ToByte(c.Substring(4, 2), 16);
            byte b = System.Convert.ToByte(c.Substring(6, 2), 16);
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        byte rr = System.Convert.ToByte(c.Substring(0, 2), 16);
        byte gg = System.Convert.ToByte(c.Substring(2, 2), 16);
        byte bb = System.Convert.ToByte(c.Substring(4, 2), 16);
            return Windows.UI.Color.FromArgb(255, rr, gg, bb);
    }

    /// <summary>Move the overlay to the target's top-right caption region.</summary>
    public void Reposition()
    {
        if (!Hittest.GetWindowRect(_target, out var rc))
        {
            return;
        }

        int capH = SystemParameters.GetSystemMetrics(NativeConstants.SM_CYCAPTION);
        int cxSize = SystemParameters.GetSystemMetrics(NativeConstants.SM_CXSIZE);
        int width = 66;
        int x = rc.right - cxSize * 3 - 8;
        int y = rc.top + Math.Max(2, (capH - 26) / 2);

        WindowStyles.Position(_self, NativeConstants.HWND_TOPMOST, x, y, width, 26, NativeConstants.SWP_NOACTIVATE);
    }

    public void HideOverlay()
    {
        WindowStyles.Position(_self, NativeConstants.HWND_TOPMOST, 0, 0, 0, 0,
            NativeConstants.SWP_HIDEWINDOW | NativeConstants.SWP_NOSIZE | NativeConstants.SWP_NOMOVE | NativeConstants.SWP_NOZORDER);
    }
}
