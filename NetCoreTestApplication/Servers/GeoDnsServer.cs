
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    // https://github.com/abh/geodns
    class GeoDnsServer
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
            } // End Using server 

        } // End Sub Test 


        static async Task OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            if (!IPAddress.IsLoopback(e.RemoteEndpoint.Address))
                e.RefuseConnect = true;
        } // End Function OnClientConnected 


        private static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;

            
            // e.RemoteEndpoint.Address

            DnsMessage response = query.CreateResponseInstance();

            // check for valid query
            if ((query.Questions.Count == 1)
                && (query.Questions[0].RecordType == RecordType.Txt)
                && (query.Questions[0].Name.Equals(ARSoft.Tools.Net.DomainName.Parse("example.com"))))
            {
                response.ReturnCode = ReturnCode.NoError;
                response.AnswerRecords.Add(new TxtRecord(ARSoft.Tools.Net.DomainName.Parse("example.com"), 3600, "Hello world"));
            }
            else
            {
                response.ReturnCode = ReturnCode.ServerFailure;
            }

            // set the response
            e.Response = response;
        } // End Function OnQueryReceived 


    } // End Class OnlyLocalRequestsServer 


} // End Namespace ArsoftTestServer 
