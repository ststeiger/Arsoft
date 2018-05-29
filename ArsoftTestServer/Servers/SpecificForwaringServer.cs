
using System.Security.Cryptography.X509Certificates;
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
        } // ENd Function CreateDnsClient 


        static Data.AnySQL s_sql;

        static SpecificForwaringServer()
        {
            s_dnsClient = CreateDnsClient();
            s_sql = Data.AnySQL.CreateInstance();

            string sqlc = s_sql.GetConnectionString();
            System.Console.WriteLine(sqlc);
        } // End Constructo 


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
            } // End if ((query.Questions.Count < 1)) 

            if (!await ResolveMessage(query, e, response))
                await ForwardMessage(query, e, response);

            // set the response
            // e.Response = response;
        } // End Function OnQueryReceived 


        public class DbDnsRecord
        {
            public long REC_Id;
            public int REC_RT_Id;
            public string REC_Name;
            public string REC_Content;
            public int? REC_TTL;
            public int? REC_Prio;
            public int? REC_Weight;
            public uint? REC_Port;
            public long? REC_ChangeDate;
            public int? REC_AfsSubType;
            public string REC_ResponsibleName;
            public uint? REC_SerialNumber;
            public int? REC_RefreshInterval; 
            public int? REC_RetryInterval ;
            public int? REC_ExpireInterval;
            public int? REC_NegativeCachingTTL;


        } // End Class DbDnsRecord 


        static async Task<bool> ResolveMessage(DnsMessage query, QueryReceivedEventArgs e, DnsMessage response)
        {
            DbDnsRecord rec = null;

            using (System.Data.Common.DbCommand cmd = s_sql.CreateCommand(@"
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
	,T_Records.REC_Weight
	,T_Records.REC_Port
	,T_Records.REC_ChangeDate
	,T_Records.REC_AfsSubType
	,T_Records.REC_ResponsibleName
	,T_Records.REC_SerialNumber
	,T_Records.REC_RefreshInterval 
	,T_Records.REC_RetryInterval 
	,T_Records.REC_ExpireInterval 
	,T_Records.REC_NegativeCachingTTL 
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
            } // End Using cmd 

            if (rec != null)
            {
                int ttl = 3600;

                DnsRecordBase record = null;

                // https://blog.dnsimple.com/2015/04/common-dns-records/
                switch ((RecordType)rec.REC_RT_Id)
                {
                    case RecordType.Soa:
                        // SoaRecord(DomainName name, int timeToLive, DomainName masterName
                        //  , DomainName responsibleName, uint serialNumber, int refreshInterval
                        //  , int retryInterval, int expireInterval, int negativeCachingTTL)
                        record = new ARSoft.Tools.Net.Dns.SoaRecord(
                                  DomainName.Parse(rec.REC_Name)
                                , ttl
                                , DomainName.Parse(rec.REC_Content)
                                , DomainName.Parse(rec.REC_ResponsibleName)
                                , rec.REC_SerialNumber.Value
                                , rec.REC_RefreshInterval.Value 
                                , rec.REC_RetryInterval.Value
                                , rec.REC_ExpireInterval.Value
                                , rec.REC_NegativeCachingTTL.Value
                        );
                        break;
                    case RecordType.Ns:
                        record = new ARSoft.Tools.Net.Dns.NsRecord(DomainName.Parse(rec.REC_Name), ttl, DomainName.Parse(rec.REC_Content));
                        break;
                    case RecordType.Srv:
                        // SrvRecord(DomainName name, int timeToLive, ushort priority, ushort weight, ushort port, DomainName target)
                        record = new ARSoft.Tools.Net.Dns.SrvRecord(DomainName.Parse(rec.REC_Name), rec.REC_TTL.Value, (ushort) rec.REC_Prio.Value, (ushort) rec.REC_Weight.Value, (ushort) rec.REC_Port.Value, DomainName.Parse(rec.REC_Content));
                        break;
                    // https://www.openafs.org/
                    // https://en.wikipedia.org/wiki/OpenAFS
                    // http://www.rjsystems.nl/en/2100-dns-discovery-openafs.php
                    // OpenAFS is an open source implementation of the Andrew distributed file system(AFS). 
                    case RecordType.Afsdb:
                        record = new ARSoft.Tools.Net.Dns.AfsdbRecord(DomainName.Parse(rec.REC_Name), rec.REC_TTL.Value, (AfsdbRecord.AfsSubType) (uint)rec.REC_AfsSubType.Value, DomainName.Parse(rec.REC_Content));
                        break;
                    // A DNS-based Authentication of Named Entities (DANE) method
                    // for publishing and locating OpenPGP public keys in DNS
                    // for a specific email address using an OPENPGPKEY DNS resource record.
                    case RecordType.OpenPGPKey:
                        byte[] publicKey = null;
                        
                        // hexdump(sha256(truncate(utf8(ocalpart), 28)
                        // https://www.huque.com/bin/openpgpkey
                        // The OPENPGPKEY DNS record is specied in RFC 7929. 
                        // The localpart of the uid is encoded as a DNS label
                        // containing the hexdump of the SHA-256 hash 
                        // of the utf-8 encoded localpart, truncated to 28 octets. 
                        // Normally the "Standard" output format should be used. 
                        // The "Generic Encoding" output format is provided to help work 
                        // with older DNS software that does not yet understand the OPENPGPKEY record type.
                        record = new ARSoft.Tools.Net.Dns.OpenPGPKeyRecord(DomainName.Parse(rec.REC_Name), ttl, publicKey);
                        break;
                    // Canonical name records, or CNAME records, are often called alias records because they map an alias to the canonical name. When a name server finds a CNAME record, it replaces the name with the canonical name and looks up the new name.
                    case RecordType.CName:
                        record = new ARSoft.Tools.Net.Dns.CNameRecord(DomainName.Parse(rec.REC_Name), ttl, DomainName.Parse(rec.REC_Content));
                        break;
                    case RecordType.Ptr:
                        record = new ARSoft.Tools.Net.Dns.PtrRecord(DomainName.Parse(rec.REC_Name), ttl, DomainName.Parse(rec.REC_Content));
                        break;
                    case RecordType.A:
                        record = new ARSoft.Tools.Net.Dns.ARecord(DomainName.Parse(rec.REC_Name), ttl, System.Net.IPAddress.Parse(rec.REC_Content));
                        break;
                    case RecordType.Aaaa:
                        record = new ARSoft.Tools.Net.Dns.AaaaRecord(DomainName.Parse(rec.REC_Name), ttl, System.Net.IPAddress.Parse(rec.REC_Content));
                        break;
                    case RecordType.Mx:
                        record = new ARSoft.Tools.Net.Dns.MxRecord(DomainName.Parse(rec.REC_Name), ttl, 0, DomainName.Parse(rec.REC_Content));
                        break;
                    case RecordType.Txt:
                        record = new ARSoft.Tools.Net.Dns.TxtRecord(DomainName.Parse(rec.REC_Name), ttl, rec.REC_Content);
                        break;
                    case RecordType.SshFp:
                        // https://unix.stackexchange.com/questions/121880/how-do-i-generate-sshfp-records
                        
                        // SshFpRecord(DomainName name, int timeToLive, SshFpAlgorithm algorithm
                        //     , SshFpFingerPrintType fingerPrintType, byte[] fingerPrint)
                        ARSoft.Tools.Net.Dns.SshFpRecord.SshFpAlgorithm sfa = ARSoft.Tools.Net.Dns.SshFpRecord
                            .SshFpAlgorithm.Rsa;
                        
                        ARSoft.Tools.Net.Dns.SshFpRecord.SshFpFingerPrintType sfp = ARSoft.Tools.Net.Dns.SshFpRecord
                            .SshFpFingerPrintType.Sha256;
                        
                        byte[] fp = null;
                        
                        record = new ARSoft.Tools.Net.Dns.SshFpRecord(DomainName.Parse(rec.REC_Name), ttl, sfa,sfp, fp);
                        break;
                    default:
                        break;
                } // End Switch 

                if (record != null)
                    response.AnswerRecords.Add(record);

                response.ReturnCode = ReturnCode.NoError;
                e.Response = response;
                return await Task<bool>.FromResult(true);
            } // End if (rec != null) 

            return await Task<bool>.FromResult(false);
        } // End Function ResolveMessage


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
