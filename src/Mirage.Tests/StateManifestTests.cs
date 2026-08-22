using Mirage.Core;
using Xunit;

namespace Mirage.Tests;

public class StateManifestTests
{
    [Fact]
    public void Record_AppendsMutation()
    {
        var manifest = new StateManifest();
        manifest.Record(new Mutation { Feature = "Dock", Operation = "ABM_NEW" });
        Assert.Single(manifest.Mutations);
    }

    [Fact]
    public void Reversed_ReturnsMutationsInUndoOrder()
    {
        var manifest = new StateManifest();
        manifest.Record(new Mutation { Feature = "A", Operation = "1" });
        manifest.Record(new Mutation { Feature = "B", Operation = "2" });
        manifest.Record(new Mutation { Feature = "C", Operation = "3" });

        var reversed = manifest.Reversed();
        Assert.Equal("C", reversed.First().Feature);
        Assert.Equal("A", reversed.Last().Feature);
    }

    [Fact]
    public void Load_WhenMissingFile_ReturnsEmpty()
    {
        StateManifest.Delete();
        var loaded = StateManifest.Load();
        Assert.Empty(loaded.Mutations);
    }
}
