using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Mirage.Core.Models;

namespace Mirage.App.Controls;

public sealed partial class DockItemControl : UserControl
{
    public DockItemControl()
    {
        this.InitializeComponent();
    }

    public RunningApp? App { get; set; }

    public void SetRunning(bool running)
    {
        RunningDot.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
    }

    public void SetBadge(int count)
    {
        if (count <= 0)
        {
            Badge.Visibility = Visibility.Collapsed;
            return;
        }

        BadgeText.Text = count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        Badge.Visibility = Visibility.Visible;
    }

    public void SetIconBrush(Brush brush)
    {
        Icon.Fill = brush;
    }
}
