
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    public class Client
    {




        // http://stackoverflow.com/questions/2669841/how-to-get-mx-records-for-a-dns-name-with-system-net-dns
        public static void GetExchangeDomainName()
        {
            DnsMessage response = DnsClient.Default.Resolve(ARSoft.Tools.Net.DomainName.Parse("gmail.com"), RecordType.Mx);

            System.Collections.Generic.IEnumerable<MxRecord> records = System.Linq.Enumerable.OfType<MxRecord>(response.AnswerRecords);
            foreach (MxRecord record in records)
            {
                System.Console.WriteLine(record.ExchangeDomainName);
            }
        }


        // Get addresses for a domain name(IPv4)
        public static void Test1()
        {
            DnsMessage dnsMessage = DnsClient.Default.Resolve(ARSoft.Tools.Net.DomainName.Parse("www.example.com"), RecordType.A);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new System.Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    ARecord aRecord = dnsRecord as ARecord;
                    if (aRecord != null)
                    {
                        System.Console.WriteLine(aRecord.Address.ToString());
                    }
                }
            }

        } // End Sub Test2 

        // Get mail exchangers for a domain name
        public static void Test2()
        {
            DnsMessage dnsMessage = DnsClient.Default.Resolve(ARSoft.Tools.Net.DomainName.Parse("example.com"), RecordType.Mx);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new System.Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    MxRecord mxRecord = dnsRecord as MxRecord;
                    if (mxRecord != null)
                    {
                        System.Console.WriteLine(mxRecord.ExchangeDomainName);
                    }
                }
            }
        } // End Sub Test2 


        // Get reverse lookup adress for an ip address
        public static void Test3()
        {
            ARSoft.Tools.Net.DomainName lookedUpDomainName = ARSoft.Tools.Net.IPAddressExtensions.GetReverseLookupDomain(
                System.Net.IPAddress.Parse("192.0.2.1")
            );

            DnsMessage dnsMessage = DnsClient.Default.Resolve(lookedUpDomainName, RecordType.Ptr);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new System.Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    PtrRecord ptrRecord = dnsRecord as PtrRecord;
                    if (ptrRecord != null)
                    {
                        System.Console.WriteLine(ptrRecord.PointerDomainName);
                    }
                }
            }
        } // End Sub Test3 


        // Send dynamic update
        public static void Test4()
        {
            ARSoft.Tools.Net.Dns.DynamicUpdate.DnsUpdateMessage msg = new ARSoft.Tools.Net.Dns.DynamicUpdate.DnsUpdateMessage()
            {
                ZoneName = ARSoft.Tools.Net.DomainName.Parse("example.com")
            };

            msg.Updates.Add(new ARSoft.Tools.Net.Dns.DynamicUpdate.DeleteRecordUpdate(ARSoft.Tools.Net.DomainName.Parse("dyn.example.com"), RecordType.A));
            msg.Updates.Add(new ARSoft.Tools.Net.Dns.DynamicUpdate.AddRecordUpdate(
                new ARecord(ARSoft.Tools.Net.DomainName.Parse("dyn.example.com"), 300, System.Net.IPAddress.Parse("192.0.2.42"))));
            msg.TSigOptions = new TSigRecord(ARSoft.Tools.Net.DomainName.Parse("my-key"), TSigAlgorithm.Md5, System.DateTime.Now
                , new System.TimeSpan(0, 5, 0), msg.TransactionID
                , ReturnCode.NoError, null
                , System.Convert.FromBase64String("0jnu3SdsMvzzlmTDPYRceA==")
            );

            ARSoft.Tools.Net.Dns.DynamicUpdate.DnsUpdateMessage dnsResult = new DnsClient(System.Net.IPAddress.Parse("192.0.2.1"), 5000).SendUpdate(msg);
        } // End Sub Test4 


    } // End Class Client 


} // End Namespace ArsoftTestServer 
