
namespace ArsoftTestServer
{


    // https://github.com/abh/geodns
    class GeoDnsServer
    {


        public static void Test()
        {
            // Org.BouncyCastle.Utilities.Net.IPAddress;

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


        private static async System.Threading.Tasks.Task OnQueryReceived(object sender, ARSoft.Tools.Net.Dns.QueryReceivedEventArgs e)
        {
            ARSoft.Tools.Net.Dns.DnsMessage query = e.Query as ARSoft.Tools.Net.Dns.DnsMessage;

            if (query == null)
                return;


            // e.RemoteEndpoint.Address

            ARSoft.Tools.Net.Dns.DnsMessage response = query.CreateResponseInstance();

            // check for valid query
            if ((query.Questions.Count == 1)
                && (query.Questions[0].RecordType == ARSoft.Tools.Net.Dns.RecordType.Txt)
                && (query.Questions[0].Name.Equals(ARSoft.Tools.Net.DomainName.Parse("example.com"))))
            {
                response.ReturnCode = ARSoft.Tools.Net.Dns.ReturnCode.NoError;
                response.AnswerRecords.Add(new ARSoft.Tools.Net.Dns.TxtRecord(ARSoft.Tools.Net.DomainName.Parse("example.com"), 3600, "Hello world"));
            }
            else
            {
                response.ReturnCode = ARSoft.Tools.Net.Dns.ReturnCode.ServerFailure;
            }

            // set the response
            e.Response = response;
        } // End Function OnQueryReceived 


    } // End Class OnlyLocalRequestsServer 


} // End Namespace ArsoftTestServer 
