using System;
using System.Runtime.InteropServices;

namespace Mirage.Native
{
    /// <summary>
    /// Small helper to compute unmanaged struct sizes without repeatedly writing
    /// the verbose Marshal call at every call site.
    /// </summary>
    internal static class MarshalHelper
    {
        public static int SizeOf<T>() where T : struct => Marshal.SizeOf<T>();

        public static int SizeOf(Type type) => Marshal.SizeOf(type);
    }
}
