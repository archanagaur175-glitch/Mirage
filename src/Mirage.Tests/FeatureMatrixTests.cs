using Mirage.Core;
using Xunit;

namespace Mirage.Tests;

public class FeatureMatrixTests
{
    [Fact]
    public void Toggles_AreIndependent()
    {
        var f = new FeatureManager();
        f.DockEnabled = true;
        f.HudEnabled = false;
        f.TrafficLightsEnabled = true;
        f.TaskbarHidden = false;
        f.ThemingApplied = true;

        Assert.True(f.DockEnabled);
        Assert.False(f.HudEnabled);
        Assert.True(f.TrafficLightsEnabled);
        Assert.False(f.TaskbarHidden);
        Assert.True(f.ThemingApplied);
    }

    [Fact]
    public void DisableAll_TurnsEveryFeatureOff()
    {
        var f = new FeatureManager
        {
            DockEnabled = true,
            HudEnabled = true,
            TrafficLightsEnabled = true,
            TaskbarHidden = true,
            ThemingApplied = true,
        };

        f.DisableAll();

        Assert.False(f.DockEnabled);
        Assert.False(f.HudEnabled);
        Assert.False(f.TrafficLightsEnabled);
        Assert.False(f.TaskbarHidden);
        Assert.False(f.ThemingApplied);
    }
}
