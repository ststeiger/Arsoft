
using System.Threading.Tasks;


using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{
    
    
    class SpecificForwaringServer
    {

        private static ARSoft.Tools.Net.Dns.DnsClient s_dnsClient;        
        
        
        
        public static ARSoft.Tools.Net.Dns.DnsClient CreateDnsClient()
        {
            // return DnsClient.Default;
        
            // https://medium.com/@nykolas.z/dns-resolvers-performance-compared-cloudflare-x-google-x-quad9-x-opendns-149e803734e5
            System.Net.IPAddress googlePrimary = System.Net.IPAddress.Parse("8.8.8.8");
            System.Net.IPAddress googleSecondary = System.Net.IPAddress.Parse("8.8.4.4");
            System.Net.IPAddress openDnsPrimary = System.Net.IPAddress.Parse("208.67.222.222");
            System.Net.IPAddress openDnsSecondary = System.Net.IPAddress.Parse("208.67.220.220");
            System.Net.IPAddress cloudflarePrimary = System.Net.IPAddress.Parse("1.1.1.1");
            System.Net.IPAddress cloudflareSecondary = System.Net.IPAddress.Parse("1.0.0.1");
            System.Net.IPAddress quad9Primary = System.Net.IPAddress.Parse("9.9.9.9");
            System.Net.IPAddress quad9Secondary = System.Net.IPAddress.Parse("149.112.112.112");
            System.Net.IPAddress cleanBrowsingPrimary = System.Net.IPAddress.Parse("185.228.168.168");
            System.Net.IPAddress cleanBrowsingSecondary = System.Net.IPAddress.Parse("185.228.168.169");
            
            // Norton, policy 1,2 & 3
            System.Net.IPAddress nortonSecurityPrimary = System.Net.IPAddress.Parse("199.85.126.10");
            System.Net.IPAddress nortonSecuritySecondary = System.Net.IPAddress.Parse("199.85.127.10");
            
            System.Net.IPAddress nortonSecPronPrimary = System.Net.IPAddress.Parse("199.85.126.20");
            System.Net.IPAddress nortonSecPronSecondary = System.Net.IPAddress.Parse("199.85.127.20");
            
            System.Net.IPAddress nortonSecPronMorePrimary = System.Net.IPAddress.Parse("199.85.126.30");
            System.Net.IPAddress nortonSecPronMoreSecondary = System.Net.IPAddress.Parse("199.85.127.30");
            
            
            System.Collections.Generic.List<System.Net.IPAddress> ls = 
                new System.Collections.Generic.List<System.Net.IPAddress>();
            
            ls.Add( new System.Net.IPAddress(System.Int64.MaxValue) );
            
            int timeout = 500;
            var x = new ARSoft.Tools.Net.Dns.DnsClient(ls, timeout);
            return x;
        }



        static SpecificForwaringServer()
        {
            s_dnsClient = CreateDnsClient();
        }
        
        
        public static void Test()
        {
            using (DnsServer server = new DnsServer(System.Net.IPAddress.Any, 10, 10))
            {
                server.ClientConnected += OnClientConnected;
                server.QueryReceived += OnQueryReceived;
                
                server.Start();
                
                System.Console.WriteLine("Press any key to stop server");
                System.Console.ReadLine();
            } // End Using server 

        } // End Sub Test 
        
        
        
        static async Task OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            // local only
            if (!System.Net.IPAddress.IsLoopback(e.RemoteEndpoint.Address))
                e.RefuseConnect = true;
        } // End Function OnClientConnected 
        
        
        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            // if (!IPAddress.IsLoopback(e.RemoteEndpoint.Address))
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;
            
            DnsMessage response = query.CreateResponseInstance();

            // check for valid query
            if ((query.Questions.Count == 1)
                && (query.Questions[0].RecordType == RecordType.Txt)
                && (query.Questions[0].Name.Equals(ARSoft.Tools.Net.DomainName.Parse("example.com"))))
            {
                response.ReturnCode = ReturnCode.NoError;
                response.AnswerRecords.Add(new TxtRecord(ARSoft.Tools.Net.DomainName.Parse("example.com"), 3600, "Hello world"));
                e.Response = response;
            }
            else
            {
                await ForwardMessage(e);
                // response.ReturnCode = ReturnCode.ServerFailure;
            }
            
            // set the response
            // e.Response = response;
        }
        
        
        static async Task ForwardMessage(QueryReceivedEventArgs e)
        {
            DnsMessage message = e.Query as DnsMessage;
            
            if (message == null)
                return;
            
            DnsMessage response = message.CreateResponseInstance();
            
            
            if ((message.Questions.Count == 1))
            {
                // send query to upstream server
                DnsQuestion question = message.Questions[0];
                
                DnsMessage upstreamResponse = await s_dnsClient.ResolveAsync(question.Name
                    , question.RecordType, question.RecordClass
                );
                
                // if got an answer, copy it to the message sent to the client
                if (upstreamResponse != null)
                {
                    foreach (DnsRecordBase record in (upstreamResponse.AnswerRecords))
                    {
                        response.AnswerRecords.Add(record);
                    } // Next record 
                    
                    foreach (DnsRecordBase record in (upstreamResponse.AdditionalRecords))
                    {
                        response.AdditionalRecords.Add(record);
                    } // Next record 
                    
                    response.ReturnCode = ReturnCode.NoError;
                    
                    // set the response
                    e.Response = response;
                } // End if (upstreamResponse != null) 
                
            } // End if ((message.Questions.Count == 1)) 
            
        } // End Function OnQueryReceived 
        
        
    } // End Class SpecificForwaringServer 
    
    
} // End Namespace ArsoftTestServer 
