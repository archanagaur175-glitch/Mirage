using System;
using Microsoft.UI.Xaml;
using Mirage.Core;
using Mirage.Core.Services;

namespace Mirage.App;

public partial class App : Application
{
    private Window? _mainWindow;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        // Decide which surfaces to show based on persisted feature state.
        var features = FeatureManager.Load();

        _mainWindow = new MainWindow();
        _mainWindow.Activate();

        if (features.DockEnabled)
        {
            DockWindow.Show();
        }

        if (features.HudEnabled)
        {
            HudWindow.Show();
        }

        if (features.TrafficLightsEnabled)
        {
            TrafficLightOverlayController.Start();
        }

        if (features.TaskbarHidden)
        {
            new TaskbarService().Hide();
        }
    }

    public static void RequestRevert()
    {
        // Close all surfaces first, then replay the manifest in reverse.
        DockWindow.CloseInstance();
        HudWindow.CloseInstance();
        TrafficLightOverlayController.Stop();

        new RevertSwitch().PerformRevert();
    }
}
