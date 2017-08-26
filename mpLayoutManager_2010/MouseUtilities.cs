using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace mpLayoutManager
{
    public class MouseUtilities
    {
        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool GetCursorPos(ref MouseUtilities.Win32Point pt);

        public static Point GetMousePosition(Visual relativeTo)
        {
            MouseUtilities.Win32Point win32Point = new MouseUtilities.Win32Point();
            MouseUtilities.GetCursorPos(ref win32Point);
            Point point = relativeTo.PointFromScreen(new Point((double)win32Point.X, (double)win32Point.Y));
            return point;
        }

        [DllImport("user32.dll", CharSet = CharSet.None, ExactSpelling = false)]
        private static extern bool ScreenToClient(IntPtr hwnd, ref MouseUtilities.Win32Point pt);

        private struct Win32Point
        {
            public int X;

            public int Y;
        }
    }
}