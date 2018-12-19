using Microsoft.Win32;
using robot.core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace robot.util
{
    public class RasName
    {
        #region 获取adsl所有宽带连接名称

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RasEntryName      //define the struct to receive the entry name
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256 + 1)]
            public string szEntryName;
#if WINVER5
     public int dwFlags;
     [MarshalAs(UnmanagedType.ByValTStr,SizeConst=260+1)]
     public string szPhonebookPath;
#endif
        }

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]

        public extern static uint RasEnumEntries(
            string reserved,              // reserved, must be NULL
            string lpszPhonebook,         // pointer to full path and file name of phone-book file
            [In, Out]RasEntryName[] lprasentryname, // buffer to receive phone-book entries
            ref int lpcb,                  // size in bytes of buffer
            out int lpcEntries             // number of entries written to buffer
        );


        public static string GetDefaultEntry()
        {
            string registData;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey microsoft = software.OpenSubKey("Microsoft", true);
            RegistryKey rasAutoDail = microsoft.OpenSubKey("RAS AutoDial", true);
            RegistryKey defaultKey = rasAutoDail.OpenSubKey("Default", true);
            if (defaultKey != null)
            {
                Object value = defaultKey.GetValue("DefaultInternet");
                if (value != null)
                {
                    return value.ToString();
                }
            }
            LogCore.Write("注册表访问失败.");
            return "";
        }

        public static string GetAdslName()
        {
            List<string> list = new List<string>();
            int lpNames = 1;
            int entryNameSize = 0;
            int lpSize = 0;
            RasEntryName[] names = null;
            entryNameSize = Marshal.SizeOf(typeof(RasEntryName));
            lpSize = lpNames * entryNameSize;
            names = new RasEntryName[lpNames];
            names[0].dwSize = entryNameSize;
            uint retval = RasEnumEntries(null, null, names, ref lpSize, out lpNames);

            //if we have more than one connection, we need to do it again
            if (lpNames > 1)
            {
                names = new RasEntryName[lpNames];
                for (int i = 0; i < names.Length; i++)
                {
                    names[i].dwSize = entryNameSize;
                }
                retval = RasEnumEntries(null, null, names, ref lpSize, out lpNames);
            }

            if (lpNames > 0)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    list.Add(names[i].szEntryName);
                }
            }

            if (list.Count>0)
            {
                if (list.Count == 1)
                {
                    return list[0];
                }
                else
                {
                    return GetDefaultEntry();
                }
            }
            return "";
        }

        #endregion
    }
}