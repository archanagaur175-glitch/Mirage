using System;
using System.Collections.Generic;
using Mirage.Native;

namespace Mirage.Core.Services
{
    /// <summary>
    /// Manages traffic-light decoration state for Mirage-owned and third-party
    /// windows. Holds the global enable flag plus a per-window disable map and an
    /// exclusion list (UWP/immersive, elevated, fullscreen). The overlay itself
    /// lives in the App project; this service only owns policy + manifest entries.
    /// </summary>
    public sealed class TrafficLightService
    {
        private readonly HashSet<IntPtr> _disabledWindows = new();
        private readonly HashSet<IntPtr> _excludedWindows = new();
        private readonly HashSet<string> _exclusions = new(StringComparer.OrdinalIgnoreCase)
        {
            // UWP/immersive and elevated windows are excluded by default (see §4.3).
            "ApplicationFrameHost.exe",
            "ShellExperienceHost.exe",
            "SearchUI.exe",
        };

        public bool IsExcluded(IntPtr hwnd)
        {
            int cloaked = Dwm.GetCloaked(hwnd);
            if (cloaked != 0)
            {
                return true;
            }

            return _excludedWindows.Contains(hwnd);
        }

        public void ExcludeWindow(IntPtr hwnd) => _excludedWindows.Add(hwnd);
        public void IncludeWindow(IntPtr hwnd) => _excludedWindows.Remove(hwnd);
        public bool IsWindowDisabled(IntPtr hwnd) => _disabledWindows.Contains(hwnd);
        public void DisableWindow(IntPtr hwnd) => _disabledWindows.Add(hwnd);
        public void EnableWindow(IntPtr hwnd) => _disabledWindows.Remove(hwnd);

        public void Enable()
        {
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "TrafficLights",
                Api = "DwmExtendFrameIntoClientArea",
                Operation = "Enable",
            });
        }

        public void Disable()
        {
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "TrafficLights",
                Api = "DwmExtendFrameIntoClientArea",
                Operation = "Disable",
            });
        }

        public IReadOnlyCollection<string> ExcludedProcesses() => _exclusions;
    }
}
