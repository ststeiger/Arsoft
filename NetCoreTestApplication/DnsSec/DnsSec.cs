
using ARSoft.Tools.Net.Dns;


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
                System.Console.WriteLine("example.com has following signed SSH fingerprint records:");
                result.Records.ForEach(x => System.Console.WriteLine(x.ToString()));
            }
            else
            {
                System.Console.WriteLine("example.com has no signed SSH fingerprint records");
            }
        } // End Sub Test1 


        // https://stackoverflow.com/questions/28612289/tcpclient-connectasync-or-socket-beginconnect-with-non-blocking-timeout-setting
        public static void Test2()
        {
            IDnsSecResolver resolver = new DnsSecRecursiveDnsResolver();
            //using (System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient("example.com", 443))

            using (System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient())
            {
                System.Threading.Tasks.Task t = client.ConnectAsync("example.com", 443);
                t.Wait();

                using (ARSoft.Tools.Net.Net.DaneStream stream = new ARSoft.Tools.Net.Net.DaneStream(client.GetStream(), resolver))
                {
                    stream.AuthenticateAsClient("example.com", 443);

                    if (stream.IsAuthenticatedByDane)
                        System.Console.WriteLine("Stream is authenticated by DANE/TLSA");

                    // work with the stream
                } // End Using stream 

            } // End Using client 

        } // End Sub Test2 


    } // End Class DnsSec 


} // End Namespace ArsoftTestServer 
