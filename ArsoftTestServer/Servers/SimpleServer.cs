
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    public class SimpleServer
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


    } // End Class SimpleServer 


} // End Namespace ArsoftTestServer 
