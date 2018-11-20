using System.Runtime.InteropServices;

namespace robot.util
{
    class MouseKeyboard
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        //鼠标操作
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;      //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标


        public static void moveTo(int x, int y)
        {
            mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE , x, y, 0, 0);
        }
    }
}
