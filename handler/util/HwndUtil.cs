using System;
using System.Runtime.InteropServices;

namespace handler.util
{
    class HwndUtil
    {
        const int WM_CLOSE = 0x0010;

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);
        [DllImport("user32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(int hwnd,string lpString,int cch);
        [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Ansi)]
        public static extern int SetWindowText(int hwnd, string lpString);

        public static void clickHwnd(IntPtr hWnd)
        {
            SendMessage(hWnd, 0xF5, IntPtr.Zero, null);
        }

        public static void setText(IntPtr hwnd,string text)
        {
            SendMessage(hwnd, 0x0C, IntPtr.Zero, text);
        }

        public static void closeHwnd(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, "");
        }
    }
}
