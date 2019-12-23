
namespace ArsoftTestServer
{


    // https://docs.ar-soft.de/arsoft.tools.net/DNS%20Server.html
    public class SimpleServer
    {


        private static async System.Threading.Tasks.Task OnQueryReceived(object sender, ARSoft.Tools.Net.Dns.QueryReceivedEventArgs e)
        {
            ARSoft.Tools.Net.Dns.DnsMessage query = e.Query as ARSoft.Tools.Net.Dns.DnsMessage;

            if (query == null)
                return;

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


        public static void Test()
        {

            using (ARSoft.Tools.Net.Dns.DnsServer server = new ARSoft.Tools.Net.Dns.DnsServer(10, 10))
            {
                server.QueryReceived += OnQueryReceived;

                server.Start();

                System.Console.WriteLine("Press any key to stop server");
                System.Console.ReadKey();
            } // End Using server 

        } // End Sub Test 


    } // End Class SimpleServer 


} // End Namespace ArsoftTestServer 
