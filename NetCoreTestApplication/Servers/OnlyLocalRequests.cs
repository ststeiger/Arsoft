
namespace ArsoftTestServer
{

    // https://docs.ar-soft.de/arsoft.tools.net/DNS%20Server.html
    class OnlyLocalRequestsServer
    {
        public static void Test()
        {
            using (ARSoft.Tools.Net.Dns.DnsServer server = new ARSoft.Tools.Net.Dns.DnsServer(System.Net.IPAddress.Any, 10, 10))
            {
                server.ClientConnected += OnClientConnected;
                server.QueryReceived += OnQueryReceived;

                server.Start();

                System.Console.WriteLine("Press any key to stop server");
                System.Console.ReadLine();
            } // End Using server 

        } // End Sub Test 


        static async System.Threading.Tasks.Task OnClientConnected(object sender, ARSoft.Tools.Net.Dns.ClientConnectedEventArgs e)
        {
            if (!System.Net.IPAddress.IsLoopback(e.RemoteEndpoint.Address))
                e.RefuseConnect = true;
        } // End Function OnClientConnected 


        static async System.Threading.Tasks.Task OnQueryReceived(object sender, ARSoft.Tools.Net.Dns.QueryReceivedEventArgs e)
        {
            // process query as you like
        } // End Function OnQueryReceived 


    } // End Class OnlyLocalRequestsServer 


} // End Namespace ArsoftTestServer 
