using System;
using System.Windows.Forms;

namespace Installer
{
    class OldWindow : IWin32Window
    {
        public OldWindow(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; }
    }
}
