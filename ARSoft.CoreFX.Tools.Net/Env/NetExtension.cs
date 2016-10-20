using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System.Net
{

    // Environment.OSVersion.Platform == PlatformID.Unix

    public static class NetExtension
    {

        public static void Close(this TcpClient tcpClient)
        {
        }

        public static void Close(this System.Threading.WaitHandle handle)
        {
        }
    }
}
