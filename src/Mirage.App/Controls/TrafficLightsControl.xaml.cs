using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Mirage.Native;
using WinRT.Interop;

namespace Mirage.App.Controls;

public sealed partial class TrafficLightsControl : UserControl
{
    public TrafficLightsControl()
    {
        this.InitializeComponent();
    }

    /// <summary>The WinUI window these lights control. Set by the host window.</summary>
    public Window? Target { get; set; }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        if (TryGetHwnd(out var hwnd))
        {
            Hittest.PostSysCommand(hwnd, NativeConstants.SC_CLOSE);
        }
    }

    private void OnMinimize(object sender, RoutedEventArgs e)
    {
        if (TryGetHwnd(out var hwnd))
        {
            Hittest.PostSysCommand(hwnd, NativeConstants.SC_MINIMIZE);
        }
    }

    private void OnMaximize(object sender, RoutedEventArgs e)
    {
        if (TryGetHwnd(out var hwnd))
        {
            Hittest.PostSysCommand(hwnd, NativeConstants.SC_MAXIMIZE);
        }
    }

    private bool TryGetHwnd(out IntPtr hwnd)
    {
        hwnd = IntPtr.Zero;
        if (Target is null)
        {
            return false;
        }

        hwnd = WindowNative.GetWindowHandle(Target);
        return hwnd != IntPtr.Zero;
    }
}
