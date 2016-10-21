using ARSoft.Tools.Net.Dns;
using ARSoft.Tools.Net.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArsoftTestServer
{
    public class DnsSec
    {

        public static void Test1()
        {
            IDnsSecResolver resolver = new SelfValidatingInternalDnsSecStubResolver();
            DnsSecResult<SshFpRecord> result = resolver.ResolveSecure<SshFpRecord>("example.com", RecordType.SshFp);
            if (result.ValidationResult == DnsSecValidationResult.Signed)
            {
                Console.WriteLine("example.com has following signed SSH fingerprint records:");
                result.Records.ForEach(x => Console.WriteLine(x.ToString()));
            }
            else
            {
                Console.WriteLine("example.com has no signed SSH fingerprint records");
            }
        }


        // https://stackoverflow.com/questions/28612289/tcpclient-connectasync-or-socket-beginconnect-with-non-blocking-timeout-setting
        public static void Test2()
        {
            IDnsSecResolver resolver = new DnsSecRecursiveDnsResolver();
            //using (TcpClient client = new TcpClient("example.com", 443))
            using (TcpClient client = new TcpClient())
            {
                Task t = client.ConnectAsync("example.com", 443);
                t.Wait();


                using (DaneStream stream = new DaneStream(client.GetStream(), resolver))
                {
                    stream.AuthenticateAsClient("example.com", 443);

                    if (stream.IsAuthenticatedByDane)
                        Console.WriteLine("Stream is authenticated by DANE/TLSA");

                    // work with the stream
                }
            }
        }


    }


}
