using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace handler.util
{
    class Computer
    {
        [DllImport("kernel32.dll", EntryPoint = "SetComputerNameEx")]
        public static extern int apiSetComputerNameEx(int type, string lpComputerName);
    }
}
