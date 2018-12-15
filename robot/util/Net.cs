using System.Runtime.InteropServices;

namespace robot.util
{
    class Net
    {
        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(
        ref int dwFlag,
        int dwReserved);
        public static bool isOnline()
        {
            System.Int32 dwFlag = new int();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                return false;
            }
            else if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
