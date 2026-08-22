using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using Mirage.Core;
using Mirage.Core.Services;
using Windows.Graphics;

namespace Mirage.App;

public sealed partial class MainWindow : Window
{
    private readonly FeatureManager _features = FeatureManager.Load();

    public MainWindow()
    {
        this.InitializeComponent();

        this.Activated += OnMainWindowActivated;

        // Custom (macOS-style) title bar drawn in the client area.
        Lights.Target = this;

        DockToggle.IsOn = _features.DockEnabled;
        HudToggle.IsOn = _features.HudEnabled;
        TrafficToggle.IsOn = _features.TrafficLightsEnabled;
        TaskbarToggle.IsOn = _features.TaskbarHidden;

        StatusText.Text = "Mirage is idle. Toggle a feature to begin.";
    }

    private void OnMainWindowActivated(object sender, WindowActivatedEventArgs e)
    {
        this.Activated -= OnMainWindowActivated;
        if (AppWindow is not null)
        {
            AppWindow.Resize(new Windows.Graphics.SizeInt32(440, 660));
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        }
    }

    private void OnDockToggled(object sender, RoutedEventArgs e)
    {
        _features.DockEnabled = DockToggle.IsOn;
        _features.Save();
        if (DockToggle.IsOn) { DockWindow.Show(); StatusText.Text = "Dock enabled."; }
        else { DockWindow.CloseInstance(); StatusText.Text = "Dock disabled."; }
    }

    private void OnHudToggled(object sender, RoutedEventArgs e)
    {
        _features.HudEnabled = HudToggle.IsOn;
        _features.Save();
        if (HudToggle.IsOn) { HudWindow.Show(); new HudService().Enable(); StatusText.Text = "Top HUD enabled."; }
        else { HudWindow.CloseInstance(); new HudService().Disable(); StatusText.Text = "Top HUD disabled."; }
    }

    private void OnTrafficToggled(object sender, RoutedEventArgs e)
    {
        _features.TrafficLightsEnabled = TrafficToggle.IsOn;
        _features.Save();
        if (TrafficToggle.IsOn) { TrafficLightOverlayController.Start(); new TrafficLightService().Enable(); StatusText.Text = "Traffic lights enabled."; }
        else { TrafficLightOverlayController.Stop(); new TrafficLightService().Disable(); StatusText.Text = "Traffic lights disabled."; }
    }

    private void OnTaskbarToggled(object sender, RoutedEventArgs e)
    {
        _features.TaskbarHidden = TaskbarToggle.IsOn;
        _features.Save();
        if (TaskbarToggle.IsOn) { new TaskbarService().Hide(); StatusText.Text = "Taskbar set to auto-hide."; }
        else { new TaskbarService().Show(); StatusText.Text = "Taskbar restored."; }
    }

    private void OnSetup(object sender, RoutedEventArgs e)
    {
        new SetupWizardWindow().Activate();
    }

    private void OnSettings(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().Activate();
    }

    private void OnRevert(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Reverting all Mirage changes...";
        App.RequestRevert();
        _features.DockEnabled = false;
        _features.HudEnabled = false;
        _features.TrafficLightsEnabled = false;
        _features.TaskbarHidden = false;
        _features.Save();
        DockToggle.IsOn = false;
        HudToggle.IsOn = false;
        TrafficToggle.IsOn = false;
        TaskbarToggle.IsOn = false;
        StatusText.Text = "Reverted. System is back to its original state.";
    }
}
