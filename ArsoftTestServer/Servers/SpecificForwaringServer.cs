
using System.Threading.Tasks;

using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{

    // https://docs.ar-soft.de/arsoft.tools.net

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

            // ls.Add(cloudflarePrimary);
            ls.Add(cloudflareSecondary);

            int timeout = 500;
            ARSoft.Tools.Net.Dns.DnsClient client = new ARSoft.Tools.Net.Dns.DnsClient(ls, timeout);
            return client;
        }


        static Data.AnySQL s_sql; 

        static SpecificForwaringServer()
        {
            s_dnsClient = CreateDnsClient();
            s_sql = Data.AnySQL.CreateInstance();

            string sqlc = s_sql.GetConnectionString();
            System.Console.WriteLine(sqlc);
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

            // await Task.CompletedTask;
            await Task.FromResult(e);
        } // End Function OnClientConnected 


        // nslookup somewhere.com some.dns.server
        // nslookup somewhere.com 127.0.0.1
        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            // if (!IPAddress.IsLoopback(e.RemoteEndpoint.Address))
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;
            
            DnsMessage response = query.CreateResponseInstance();

            if ((query.Questions.Count < 1))
            {
                response.ReturnCode = ReturnCode.NoError;
                return;
            }

            if(!await ResolveMessage(query, e, response))
                await ForwardMessage(query, e, response);

            // set the response
            // e.Response = response;
        }
        

        public class DbDnsRecord
        {
            public long REC_Id;
            public int REC_RT_Id;
            public string REC_Name;
            public string REC_Content;
            public int? REC_TTL;
            public int? REC_Prio;
            public long? REC_ChangeDate;
        }


        static async Task<bool> ResolveMessage(DnsMessage query, QueryReceivedEventArgs e, DnsMessage response)
        {
            DbDnsRecord rec = null;


            using (var cmd = s_sql.CreateCommand(@"
-- DECLARE @in_recordType int 
-- DECLARE @in_recordName varchar(4000) 

-- SET @in_recordType = 1 -- A 
-- SET @in_recordName = 'vortex.data.microsoft.com' 


SELECT 
	 T_Records.REC_Id
	--,T_Records.REC_DOM_Id
	,T_Records.REC_RT_Id
	,T_Records.REC_Name
	,T_Records.REC_Content
	,ISNULL(T_Records.REC_TTL, 100) AS REC_TTL 
	,T_Records.REC_Prio
	,T_Records.REC_ChangeDate
FROM T_Records
WHERE REC_RT_Id	= @in_recordType 
AND T_Records.REC_Name = @in_recordName 
;
"))
            {
                try
                {
                    string name = query.Questions[0].Name.ToString();

                    s_sql.AddParameter(cmd, "in_recordType", (int)query.Questions[0].RecordType);
                    s_sql.AddParameter(cmd, "in_recordName", name);

                    // TODO: Can return multiple records...
                    rec = s_sql.GetClass<DbDnsRecord>(cmd);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
            }

            if (rec != null)
            {
                int ttl = 3600;

                DnsRecordBase record = null;

                switch ((RecordType)rec.REC_RT_Id)
                {
                    case RecordType.A:
                        record = new ARSoft.Tools.Net.Dns.ARecord(DomainName.Parse(rec.REC_Name), ttl, System.Net.IPAddress.Parse(rec.REC_Content));
                        break;
                    case RecordType.Txt:
                        record = new ARSoft.Tools.Net.Dns.TxtRecord(DomainName.Parse(rec.REC_Name), ttl, rec.REC_Content);
                        break;
                    default:
                        break;
                }

                if (record != null)
                    response.AnswerRecords.Add(record);

                response.ReturnCode = ReturnCode.NoError;
                e.Response = response;
                return await Task<bool>.FromResult(true);
            }

            return await Task<bool>.FromResult(false);
        }


        static async Task ForwardMessage(DnsMessage query, QueryReceivedEventArgs e, DnsMessage response)
        {
            if ((query.Questions.Count == 1))
            {

                // send query to upstream server
                DnsQuestion question = query.Questions[0];

                DnsMessage upstreamResponse = await s_dnsClient.ResolveAsync(question.Name
                    , question.RecordType, question.RecordClass
                );

                e.Response = upstreamResponse;
                return;


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
