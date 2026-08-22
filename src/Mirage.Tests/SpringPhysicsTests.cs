using Mirage.Core;
using Xunit;

namespace Mirage.Tests;

public class SpringPhysicsTests
{
    [Fact]
    public void TargetScale_IsBaseBeyondInfluenceRadius()
    {
        double scale = SpringPhysics.TargetScale(200, 140, 1.8);
        Assert.Equal(1.0, scale, 6);
    }

    [Fact]
    public void TargetScale_PeaksAtCenter()
    {
        double center = SpringPhysics.TargetScale(0, 140, 1.8);
        double edge = SpringPhysics.TargetScale(139, 140, 1.8);
        Assert.True(center > edge);
        Assert.True(center <= 1.8);
    }

    [Fact]
    public void Spring_ConvergesToTarget()
    {
        var spring = new SpringPhysics.Spring();
        spring.Settle(1.0);

        for (int i = 0; i < 200; i++)
        {
            spring.Step(1.8, 120.0, 14.0, 1.0 / 60.0);
        }

        Assert.True(Math.Abs(spring.Position - 1.8) < 0.05);
    }
}
