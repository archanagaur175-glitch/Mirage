using System.Collections.Generic;
using Mirage.Core.Models;
using Mirage.Native;

namespace Mirage.Core.Services
{
    /// <summary>
    /// Owns Dock lifecycle: reserves the bottom edge as a normal (non-autohide)
    /// appbar via SHAppBarMessage, and discovers running applications for the
    /// indicators. The Dock never requests auto-hide itself (the real taskbar
    /// does, underneath) so there is no edge conflict.
    /// </summary>
    public sealed class DockService
    {
        public void Register(IntPtr dockHwnd, int edge, RECT rc, int callbackMessage)
        {
            AppBar.Register(dockHwnd, callbackMessage, edge, rc);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Dock",
                Api = "SHAppBarMessage",
                Operation = "ABM_NEW",
                NewValue = $"edge={edge}",
            });
        }

        public void SetPosition(IntPtr dockHwnd, int edge, RECT rc)
        {
            AppBar.SetPosition(dockHwnd, edge, rc);
        }

        public void Remove(IntPtr dockHwnd)
        {
            AppBar.Remove(dockHwnd);
            StateManifest.Instance.Record(new Mutation
            {
                Feature = "Dock",
                Api = "SHAppBarMessage",
                Operation = "ABM_REMOVE",
            });
        }

        public IReadOnlyList<RunningApp> EnumerateRunningApps()
        {
            var apps = new List<RunningApp>();
            uint self = (uint)Environment.ProcessId;
            foreach (var hwnd in WindowEnumerator.Enumerate())
            {
                uint pid;
                Hittest.GetWindowThreadProcessId(hwnd, out pid);
                if (pid == 0 || pid == self)
                {
                    continue;
                }

                var title = Hittest.GetWindowTitle(hwnd);
                if (string.IsNullOrWhiteSpace(title))
                {
                    continue;
                }

                apps.Add(new RunningApp
                {
                    Handle = hwnd,
                    ProcessId = pid,
                    Title = title,
                });
            }

            return apps;
        }
    }
}
