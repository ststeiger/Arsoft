using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArsoftTestServer
{
    public class Program
    {


        public static void Main(string[] args)
        {
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(byte[] rawData);
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = DotNetUtilities.CreateX509Cert2("mycert");
            // SecurityKey secKey = new X509SecurityKey(cert2);






            ArsoftTestServer.Resolvers.Test4();
            SimpleServer.Test();

            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
    }
}
