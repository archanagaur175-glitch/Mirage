using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Mirage.Native;
using Windows.Devices.Power;
using WinRT.Interop;

namespace Mirage.App;

public sealed partial class HudWindow : Window
{
    private static HudWindow? _instance;
    private readonly DispatcherTimer _timer = new();

    public HudWindow()
    {
        this.InitializeComponent();
        var hwnd = WindowNative.GetWindowHandle(this);

        // Topmost + tool-window only (no WS_EX_LAYERED — see DockWindow for why).
        int ex = WindowStyles.GetExStyle(hwnd);
        WindowStyles.SetExStyle(hwnd, ex | NativeConstants.WS_EX_TOPMOST | NativeConstants.WS_EX_TOOLWINDOW);

        this.SystemBackdrop = new DesktopAcrylicBackdrop();

        PositionAtTop();

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => Update();
        _timer.Start();
        Update();
    }

    private void PositionAtTop()
    {
        int screenW = SystemParameters.GetSystemMetrics(NativeConstants.SM_CXSCREEN);
        WindowStyles.Position(WindowNative.GetWindowHandle(this), NativeConstants.HWND_TOPMOST, 0, 0, screenW, 36,
            NativeConstants.SWP_NOACTIVATE);
    }

    private void Update()
    {
        ClockText.Text = DateTime.Now.ToString("ddd MMM d  h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

        try
        {
            var report = Battery.AggregateBattery.GetReport();
            var remaining = report.RemainingCapacityInMilliwattHours;
            var full = report.FullChargeCapacityInMilliwattHours;
            if (full.HasValue && full.Value > 0 && remaining.HasValue)
            {
                int pct = (int)(100.0 * remaining.Value / full.Value);
                BatteryText.Text = $"{pct}%";
            }
            else
            {
                BatteryText.Text = "Battery";
            }
        }
        catch
        {
            BatteryText.Text = "Battery";
        }
    }

    public static void Show()
    {
        if (_instance is null)
        {
            _instance = new HudWindow();
        }

        _instance.Activate();
    }

    public static void CloseInstance()
    {
        if (_instance is not null)
        {
            _instance.Close();
            _instance = null;
        }
    }
}
