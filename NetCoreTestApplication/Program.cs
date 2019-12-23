
using ARSoft.Tools.Net.Dns;
using ArsoftTestServer;

namespace NetCoreTestApplication
{
    using ARSoft.Tools.Net;
    using ARSoft.Tools.Net.Dns;
    
    class Program
    {


        public static byte[] Base64ToByteArray(string base64encoded)
        {
            base64encoded = System.Text.RegularExpressions.Regex.Replace(base64encoded, @"\s+", "");
            byte[] bytes = System.Convert.FromBase64String(base64encoded);
            return bytes;
        }


        static void Main(string[] args)
        {
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(byte[] rawData);
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = DotNetUtilities.CreateX509Cert2("mycert");
            // SecurityKey secKey = new X509SecurityKey(cert2);
            
            // https://tools.ietf.org/html/rfc4034
            // https://www.dynu.com/Resources/DNS-Records/DNSKEY-Record

            DnsRecordBase drb = null;

            DnsMessage msg = DnsMessage.Parse(new byte[]{});
            
            
            // new RrSigRecord(
            
            
            
            DsRecord dsRec = new DsRecord(
                DomainName.Parse(
                    "example.com") // Name: It defines the hostname of a record and whether the hostname will be appended to the label. 
                // Fully qualified hostnames terminated by a period will not append the origin.
                , RecordClass.Any
                , 60 // ttl The time-to-live in seconds. It specifies how long a resolver is supposed to cache or remember the DNS query 
                // before the query expires and a new one needs to be done.
                , 0 // Key Tag: A short numeric value which can help quickly identify the referenced DNSKEY-record.
                , DnsSecAlgorithm.RsaSha256 // The algorithm of the referenced DNSKEY-record.
                , DnsSecDigestType.Sha256 // Digest Type: Cryptographic hash algorithm used to create the Digest value.
                , new byte[] {1, 2, 3} // A cryptographic hash value of the referenced DNSKEY-record.
            );
            
            
            DnsKeyRecord rec = new DnsKeyRecord(
                DomainName.Parse(
                    "example.com") // Name: It defines the hostname of a record and whether the hostname will be appended to the label. 
                // Fully qualified hostnames terminated by a period will not append the origin.
                , RecordClass.Any
                , 60 // ttl The time-to-live in seconds. It specifies how long a resolver is supposed to cache or remember the DNS query 
                // before the query expires and a new one needs to be done.
                , DnsKeyFlags.Zone
                , 3 // Fixed value of 3 (for backwards compatibility)
                , DnsSecAlgorithm.RsaSha256 // The public key's cryptographic algorithm.
                , new byte[] {1, 2, 3} // Public key data.
            );
            
            // rec.Algorithm

            string key = @"AQPSKmynfzW4kyBv015MUG2DeIQ3
              Cbl+BBZH4b/0PY1kxkmvHjcZc8no
              kfzj31GajIQKY+5CptLr3buXA10h
              WqTkF7H6RfoRqXQeogmMHfpftf6z
              Mv1LyBUgia7za6ZEzOJBOztyvhjL
              742iU/TpPSEDhm2SNKLijfUppn1U
              aNvv4w== ";
            
            
            byte[] keyBytes = Base64ToByteArray(key);



            string signature = @"2BB183AF5F22588179A53B0A98631FAD1A292118";
            
            
            // ArsoftTestServer.KeyConversion.fromPublicKey()
            KEYBase keyRecord = new KEYBase(keyBytes, (int)DnsSecAlgorithm.RsaSha1);
            PublicKey pk = ArsoftTestServer.KeyConversionTo.toPublicKey(keyRecord);
            System.Console.WriteLine(pk);
            
            // ArsoftTestServer.Resolvers.Test4();
            // ArsoftTestServer.SimpleServer.Test();
            
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
        
        
    }
    
    
}
