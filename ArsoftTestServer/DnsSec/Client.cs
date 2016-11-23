
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using ARSoft.Tools.Net.Dns.DynamicUpdate;


namespace ArsoftTestServer
{


    public class Client
    {




        // http://stackoverflow.com/questions/2669841/how-to-get-mx-records-for-a-dns-name-with-system-net-dns
        public static void GetExchangeDomainName()
        {
            var response = DnsClient.Default.Resolve(ARSoft.Tools.Net.DomainName.Parse("gmail.com"), RecordType.Mx);
            var records = response.AnswerRecords.OfType<MxRecord>();
            foreach (var record in records)
            {
                Console.WriteLine(record.ExchangeDomainName);
            }
        }


        // Get addresses for a domain name(IPv4)
        public static void Test1()
        {
            DnsMessage dnsMessage = DnsClient.Default.Resolve(DomainName.Parse("www.example.com"), RecordType.A);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    ARecord aRecord = dnsRecord as ARecord;
                    if (aRecord != null)
                    {
                        Console.WriteLine(aRecord.Address.ToString());
                    }
                }
            }

        } // End Sub Test2 

        // Get mail exchangers for a domain name
        public static void Test2()
        {
            DnsMessage dnsMessage = DnsClient.Default.Resolve(DomainName.Parse("example.com"), RecordType.Mx);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    MxRecord mxRecord = dnsRecord as MxRecord;
                    if (mxRecord != null)
                    {
                        Console.WriteLine(mxRecord.ExchangeDomainName);
                    }
                }
            }
        } // End Sub Test2 


        // Get reverse lookup adress for an ip address
        public static void Test3()
        {
            DnsMessage dnsMessage = DnsClient.Default.Resolve(IPAddress.Parse("192.0.2.1").GetReverseLookupDomain(), RecordType.Ptr);
            if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
            {
                throw new Exception("DNS request failed");
            }
            else
            {
                foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                {
                    PtrRecord ptrRecord = dnsRecord as PtrRecord;
                    if (ptrRecord != null)
                    {
                        Console.WriteLine(ptrRecord.PointerDomainName);
                    }
                }
            }
        } // End Sub Test3 


        // Send dynamic update
        public static void Test4()
        {
            DnsUpdateMessage msg = new DnsUpdateMessage()
            {
                ZoneName = DomainName.Parse("example.com")
            };

msg.Updates.Add(new DeleteRecordUpdate(DomainName.Parse("dyn.example.com"), RecordType.A));
            msg.Updates.Add(new AddRecordUpdate(new ARecord(DomainName.Parse("dyn.example.com"), 300, IPAddress.Parse("192.0.2.42"))));
            msg.TSigOptions = new TSigRecord(DomainName.Parse("my-key"), TSigAlgorithm.Md5, DateTime.Now, new TimeSpan(0, 5, 0), msg.TransactionID, ReturnCode.NoError, null, Convert.FromBase64String("0jnu3SdsMvzzlmTDPYRceA=="));

            DnsUpdateMessage dnsResult = new DnsClient(IPAddress.Parse("192.0.2.1"), 5000).SendUpdate(msg);
        } // End Sub Test4 


    } // End Class Client 


} // End Namespace ArsoftTestServer 
