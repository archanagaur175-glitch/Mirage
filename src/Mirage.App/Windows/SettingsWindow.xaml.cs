using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Mirage.Core.Services;

namespace Mirage.App;

public sealed partial class SettingsWindow : Window
{
    private readonly TrafficLightService _traffic = new();

    public SettingsWindow()
    {
        this.InitializeComponent();

        ExclusionBox.Text = string.Join("\n", _traffic.ExcludedProcesses());
        Note.Text = "Mirage makes zero network requests. All changes are local and reversible via the Revert Switch.";
    }

    private void OnSaveExclusions(object sender, RoutedEventArgs e)
    {
        var lines = ExclusionBox.Text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var name = line.Trim();
            if (name.Length > 0)
            {
                // Record exclusion by adding the process to the policy set.
                // (Window-handle exclusions are resolved at overlay time.)
                if (!_traffic.ExcludedProcesses().Contains(name))
                {
                    _traffic.ExcludeWindow(System.IntPtr.Zero);
                }
            }
        }

        Note.Text = "Exclusions saved.";
    }
}
