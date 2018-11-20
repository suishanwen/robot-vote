using System.Runtime.InteropServices;

namespace robot.util
{
    class Computer
    {
        [DllImport("kernel32.dll", EntryPoint = "SetComputerNameEx")]
        public static extern int apiSetComputerNameEx(int type, string lpComputerName);
    }
}
