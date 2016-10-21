
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{

    // https://docs.ar-soft.de/arsoft.tools.net/DNS%20Server.html
    class OnlyLocalRequestsServer
    {
        public static void Test()
        {
            using (DnsServer server = new DnsServer(IPAddress.Any, 10, 10))
            {
                server.ClientConnected += OnClientConnected;
                server.QueryReceived += OnQueryReceived;

                server.Start();

                Console.WriteLine("Press any key to stop server");
                Console.ReadLine();
            }
        }

        static async Task OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (!IPAddress.IsLoopback(e.RemoteEndpoint.Address))
                e.RefuseConnect = true;
        }

        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            // process query as you like
        }
    }
}
