
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    // http://www.internetsociety.org/deploy360/resources/dnssec-developer-libraries/
    // http://www.internetsociety.org/deploy360/resources/step-by-step-how-to-use-a-dnssec-ds-record-to-link-a-registar-to-a-dns-hosting-provider/
    // http://stackoverflow.com/questions/36626648/how-to-correctly-generate-an-rrsig-record-for-a-dnssec-response
    // https://www.digitalocean.com/community/tutorials/how-to-setup-dnssec-on-an-authoritative-bind-dns-server--2

    // DLV - contains a Delegation Lookaside Validation record, 
    // http://www.dnsjava.org/doc/index.html?org/xbill/DNS/DLVRecord.html
    // http://www.dnsjava.org
    // https://tools.ietf.org/html/rfc4431
    // https://docs.ar-soft.de/arsoft.tools.net/ARSoft.Tools.Net~ARSoft.Tools.Net.Dns_namespace.html


    // https://arsofttoolsnet.codeplex.com/discussions/266145
    // http://panel.arsoft.web.tr/kb/answer/1909
    // http://www.internetsociety.org/deploy360/blog/category/dnssec/tutorials-dnssec/?gclid=CObr_tz0vtACFbIW0wodlcMJiw
    public class DnsSecServer
    {


        private static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;

            DnsMessage response = query.CreateResponseInstance();
            

            response.AnswerRecords.Add(
                new DsRecord(
                      DomainName.Parse("example.com")
                    , RecordClass.Any
                    , 60 // ttl
                    , 0 // keyTag
                    , DnsSecAlgorithm.RsaSha256
                    , DnsSecDigestType.Sha256
                    , new byte[] { 1, 2, 3 }
                )
            );


            response.AnswerRecords.Add(
                new DnsKeyRecord(
                      DomainName.Parse("example.com")
                    , RecordClass.Any
                    , 60
                    , DnsKeyFlags.Zone
                    , 3
                    , DnsSecAlgorithm.RsaSha256
                    , new byte[] { 1, 2, 3 }
                )

            );


            response.AnswerRecords.Add(
                new RrSigRecord(
                      DomainName.Parse("example.com")
                    , RecordClass.Any
                    , 60
                    , RecordType.A
                    , DnsSecAlgorithm.RsaSha256
                    , 4
                    , 0
                    , DateTime.Now.AddMinutes(1)
                    , DateTime.Now
                    , 0
                    , DomainName.Parse("example.com")
                    , new byte[] { 1, 2, 3 }
                )
            );




            // check for valid query
            if ((query.Questions.Count == 1)
                && (query.Questions[0].RecordType == RecordType.Txt)
                && (query.Questions[0].Name.Equals(DomainName.Parse("example.com"))))
            {
                response.ReturnCode = ReturnCode.NoError;
                response.AnswerRecords.Add(new TxtRecord(DomainName.Parse("example.com"), 3600, "Hello world"));
            }
            else
            {
                response.ReturnCode = ReturnCode.ServerFailure;
            }

            // set the response
            e.Response = response;
        } // End Function OnQueryReceived 


        public static void Test()
        {
            
            using (DnsServer server = new DnsServer(10, 10))
            {
                server.QueryReceived += OnQueryReceived;

                server.Start();

                Console.WriteLine("Press any key to stop server");
                System.Console.ReadKey();
            } // End Using server 

        } // End Sub Test 




        public ARSoft.Tools.Net.DomainName SignersName { get; private set; }

        public byte[] Signature { get; internal set; }

        // protected internal int MaximumRecordDataLength => 20 + SignersName.MaximumRecordDataLength + Signature.Length;
        internal int MaximumRecordDataLength
        {
            get
            {
                // private readonly string[] _labels; return _labels.Length + _labels.Sum(x => x.Length);
                //return SignersName.LabelCount + _labels.Sum(x => x.Length);
                return SignersName.ToString().Length;
            }
        }


        // https://arsofttoolsnet.codeplex.com/discussions/654851
        static void TestUpdateAdd(System.Net.IPAddress dnsServerIP,
                             ARSoft.Tools.Net.DomainName updateZoneName,
                             ARSoft.Tools.Net.DomainName newRecordName,
                             System.Net.IPAddress newRecordIPAddress,
                             ARSoft.Tools.Net.DomainName tsigName,
                             byte[] tsigKey)
        {

            var client = new DnsClient(dnsServerIP, 150000);
            client.IsUdpEnabled = false;

            var msg = new ARSoft.Tools.Net.Dns.DynamicUpdate.DnsUpdateMessage { ZoneName = updateZoneName };

            msg.Updates.Add(
                new ARSoft.Tools.Net.Dns.DynamicUpdate.AddRecordUpdate(
                    new ARecord(newRecordName, 86400, newRecordIPAddress)));

            msg.TSigOptions = new TSigRecord(tsigName, TSigAlgorithm.Md5, DateTime.Now, new TimeSpan(0, 0, 5),
                msg.TransactionID, ReturnCode.NoError, null, tsigKey);

            ARSoft.Tools.Net.Dns.DynamicUpdate.DnsUpdateMessage dnsResult = client.SendUpdate(msg);
            if (dnsResult == null)
            {
                Console.WriteLine("Failed sending message");
            }
            else
            {
                Console.WriteLine(dnsResult.ReturnCode); // FormatError
            }
        }



        private void EncodeSigningBuffer<T>(List<T> records, out byte[] messageData, out int length)
            where T : DnsRecordBase
        {
            messageData = null;
            length = 123;

            // messageData = new byte[2 + MaximumRecordDataLength - Signature.Length + records.Sum(x => x.MaximumLength)];
            // length = 0;
            //EncodeRecordData(messageData, 0, ref length, null, true, false);
            //foreach (var record in records.OrderBy(x => x))
            //{
            //    if (record.Name.LabelCount == Labels)
            //    {
            //        DnsMessageBase.EncodeDomainName(messageData, 0, ref length, record.Name, null, true);
            //    }
            //    else if (record.Name.LabelCount > Labels)
            //    {
            //        DnsMessageBase.EncodeDomainName(messageData, 0, ref length, DomainName.Asterisk + record.Name.GetParentName(record.Name.LabelCount - Labels), null, true);
            //    }
            //    else
            //    {
            //        throw new Exception("Encoding of records with less labels than RrSigRecord is not allowed");
            //    }
            // DnsMessageBase.EncodeUShort(messageData, ref length, (ushort)record.RecordType);
            // DnsMessageBase.EncodeUShort(messageData, ref length, (ushort)record.RecordClass);
            // DnsMessageBase.EncodeInt(messageData, ref length, OriginalTimeToLive);

            // record.EncodeRecordBody(messageData, 0, ref length, null, true);
        }
    }
    /*
    internal RrSigRecord(List<DnsRecordBase> records, DnsKeyRecord key, DateTime inception, DateTime expiration)
        : base(records[0].Name, RecordType.RrSig, records[0].RecordClass, records[0].TimeToLive)
    {
        TypeCovered = records[0].RecordType;
        Algorithm = key.Algorithm;
        Labels = (byte) (records[0].Name.Labels[0] == DomainName.Asterisk.Labels[0] ? records[0].Name.LabelCount - 1 : records[0].Name.LabelCount);
        OriginalTimeToLive = records[0].TimeToLive;
        SignatureExpiration = expiration;
        SignatureInception = inception;
        KeyTag = key.CalculateKeyTag();
        SignersName = key.Name;
        Signature = new byte[] { };

        byte[] signBuffer;
        int signBufferLength;
        EncodeSigningBuffer(records, out signBuffer, out signBufferLength);

        Signature = key.Sign(signBuffer, signBufferLength);
    }
    */


} // End Class DnsSecServer 


} // End Namespace ArsoftTestServer 
