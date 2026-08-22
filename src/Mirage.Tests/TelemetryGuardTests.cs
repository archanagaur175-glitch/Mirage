using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Mirage.Tests;

/// <summary>
/// Enforces the zero-telemetry trust rule: the runtime app project must not
/// reference any networking or telemetry API. This is the test-level half of the
/// guard (build.yml also greps for the same tokens).
/// </summary>
public class TelemetryGuardTests
{
    private static readonly string[] Forbidden =
    {
        "System.Net", "HttpClient", "WebRequest", "WebClient", "HttpWebRequest",
        "TcpClient", "UdpClient", "SmtpClient", "Socket", "RestSharp", "Flurl",
        "ApplicationInsights", "Telemetry", "Analytics", "Microsoft.AppCenter",
        "DownloadString", "UploadString", "WebBrowser",
    };

    [Fact]
    public void AppProject_HasNoNetworkingOrTelemetryApis()
    {
        string root = FindRepoRoot();
        Assert.NotNull(root);

        string appDir = Path.Combine(root!, "src", "Mirage.App");
        Assert.True(Directory.Exists(appDir), "Mirage.App project not found for scan.");

        var violations = new List<string>();
        foreach (var file in Directory.EnumerateFiles(appDir, "*.cs", SearchOption.AllDirectories))
        {
            string text = File.ReadAllText(file);
            foreach (var token in Forbidden)
            {
                if (text.Contains(token, StringComparison.OrdinalIgnoreCase))
                {
                    violations.Add($"{file} -> {token}");
                }
            }
        }

        Assert.Empty(violations);
    }

    private static string? FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "src", "Mirage.App")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        return null;
    }
}
