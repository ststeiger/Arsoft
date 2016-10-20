using System.Runtime.InteropServices;

namespace System
{
    // [ComVisible(true)]
    // [Serializable]
    public enum PlatformID
    {
        Win32S = 0,
        Win32Windows = 1,
        Win32NT = 2,
        WinCE = 3,
        Unix = 4,
        // Since NET 3.5 SP1 or silverlight
        Xbox = 5,
        MacOSX = 6,
    }
}
