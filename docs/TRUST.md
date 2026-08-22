# Trust & Telemetry Audit

Mirage is built to make **zero external network requests** and to contain **no
telemetry**. This document records where naive implementations leak and how Mirage
avoids each.

## Attack surfaces and mitigations

| Risk | Mitigation in Mirage |
|---|---|
| Crash reporters (AppCenter, Sentry, Bugsnag, Application Insights) | None referenced. The App project has **no** such package. |
| Analytics SDKs | None added. |
| Auto-update / phone-home checkers | None. No network path exists in this build. |
| Transitive telemetry in NuGet packages | Only `Microsoft.WindowsAppSDK` (WinUI 3) and `Microsoft.NET.Test.Sdk`/`xunit` (tests) are referenced. No telemetry-emitting package is used. |
| Compiler / SDK telemetry | CI sets `DOTNET_CLI_TELEMETRY_OPTOUT=1` and `DOTNET_NOLOGO=1`. |
| Accidental `HttpClient`/`System.Net` | Forbidden. `build.yml` **greps** `src/Mirage.App` for these tokens and fails the build on any match. `TelemetryGuardTests` asserts the same offline. |

## Result

There is intentionally **no `HttpClient`, `System.Net`, socket, or telemetry type
anywhere in the runtime app**. Any future network path must be opt-in, disabled by
default, and isolated, and would require modifying the CI guard first.
