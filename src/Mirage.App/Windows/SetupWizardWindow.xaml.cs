using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using Mirage.Core;
using Mirage.Core.Services;

namespace Mirage.App;

public sealed partial class SetupWizardWindow : Window
{
    private readonly ThemingService _theming = new();
    private readonly FeatureManager _features = FeatureManager.Load();

    public SetupWizardWindow()
    {
        this.InitializeComponent();
        AppWindow?.Resize(new SizeInt32(560, 620));
    }

    private void OnApply(object sender, RoutedEventArgs e)
    {
        if (WallpaperToggle.IsOn)
        {
            string folder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mirage", "Wallpapers");
            string path = System.IO.Path.Combine(folder, "tahoe.bmp");
            AssetFactory.WriteGradientWallpaper(path);
            _theming.SetWallpaper(path);
            Status.Text = "Wallpaper applied (original, license-safe gradient). ";
        }

        if (FontToggle.IsOn)
        {
            _theming.SetNonClientFont("Segoe UI Variable");
            Status.Text += "Font set to system Segoe UI Variable. ";
        }

        if (CursorToggle.IsOn)
        {
            _theming.SetCursorScheme("MirageCursors");
            Status.Text += "Cursor scheme recorded. ";
        }

        _features.ThemingApplied = true;
        _features.Save();
        Status.Text += "Done. Revert anytime from the main window.";
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
