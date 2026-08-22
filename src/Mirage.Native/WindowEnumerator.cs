using System;
using System.Collections.Generic;

namespace Mirage.Native
{
    /// <summary>
    /// Enumerates top-level windows (read-only). Used by the Dock to discover
    /// running applications and by the traffic-light overlay to find candidate
    /// target windows. No window is modified during enumeration.
    /// </summary>
    public static class WindowEnumerator
    {
        public static List<IntPtr> Enumerate()
        {
            var result = new List<IntPtr>();
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                result.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return result;
        }
    }
}
