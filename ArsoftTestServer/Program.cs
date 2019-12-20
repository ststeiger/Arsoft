
namespace ArsoftTestServer
{


    public class Program
    {


        public static void Main(string[] args)
        {
            // https://tools.ietf.org/html/rfc3110
            
            // https://tools.ietf.org/html/rfc2539
            // https://tools.ietf.org/html/rfc3110
            // https://tools.ietf.org/html/rfc3757
            // https://www.rfc-editor.org/rfc/rfc4034.txt
            // https://tools.ietf.org/html/rfc4034#appendix-A.1

            
            // https://metebalci.com/blog/a-minimum-complete-tutorial-of-dnssec/
            // https://www.icann.org/news/blog/changing-the-keys-to-the-domain-name-system-dns-root-zone
            // https://blog.cloudflare.com/dnssec-an-introduction/
            // https://bytesarena.com/dns/dnssec/2019/02/19/dnssec-keys-and-signing-explained.html
            // https://www.nic.ch/faqs/dnssec/details/
            // https://tools.ietf.org/html/rfc2535
            
            
            
            string foo = @"AQPSKmynfzW4kyBv015MUG2DeIQ3
            Cbl+BBZH4b/0PY1kxkmvHjcZc8no
            kfzj31GajIQKY+5CptLr3buXA10h
                WqTkF7H6RfoRqXQeogmMHfpftf6z
            Mv1LyBUgia7za6ZEzOJBOztyvhjL
            742iU/TpPSEDhm2SNKLijfUppn1U
            aNvv4w== ";


            foo = System.Text.RegularExpressions.Regex.Replace(foo, @"\s+", "");
            byte[] b64 = System.Convert.FromBase64String(foo);

            foo = System.Text.Encoding.UTF8.GetString(b64);
            System.Console.WriteLine(foo);


            
            
            
            
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(byte[] rawData);
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = DotNetUtilities.CreateX509Cert2("mycert");
            // SecurityKey secKey = new X509SecurityKey(cert2);


            ArsoftTestServer.SpecificForwaringServer.Test();


            // ArsoftTestServer.Resolvers.Test4();
            // ArsoftTestServer.SimpleServer.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}
