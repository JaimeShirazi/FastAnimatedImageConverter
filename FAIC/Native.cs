using System.Runtime.InteropServices;

namespace FAIC
{
    internal static class Native
    {
        public const int MF_SEPARATOR = 0x800;
        public const int MF_STRING = 0x0;
        public const int WM_SYSCOMMAND = 0x112;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool AppendMenu(
            IntPtr hMenu,
            int uFlags,
            int uIDNewItem,
            string lpNewItem
        );
    }
}
