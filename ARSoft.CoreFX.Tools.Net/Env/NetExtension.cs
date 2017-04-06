
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace System
{

    // Environment.OSVersion.Platform == PlatformID.Unix

    public sealed class DotNetUtilities
    {

        public static System.Security.Cryptography.X509Certificates.X509Certificate2 tox(Org.BouncyCastle.X509.X509Certificate bouncyCert)
        {
            byte[] ba = bouncyCert.GetEncoded();
            //return new System.Security.Cryptography.X509Certificates.X509Certificate(ba);
            return new System.Security.Cryptography.X509Certificates.X509Certificate2(ba);
        }


        // http://stackoverflow.com/questions/36942094/how-can-i-generate-a-self-signed-cert-without-using-obsolete-bouncycastle-1-7-0
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 CreateX509Cert2(string certName)
        {

            var keypairgen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keypairgen.Init(new Org.BouncyCastle.Crypto.KeyGenerationParameters(
                new Org.BouncyCastle.Security.SecureRandom(
                    new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
                    )
                    , 1024
                    )
            );

            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keypair = keypairgen.GenerateKeyPair();

            // --- Until here we generate a keypair



            var random = new Org.BouncyCastle.Security.SecureRandom(
                    new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
            );


            // SHA1WITHRSA
            // SHA256WITHRSA
            // SHA384WITHRSA
            // SHA512WITHRSA

            // SHA1WITHECDSA
            // SHA224WITHECDSA
            // SHA256WITHECDSA
            // SHA384WITHECDSA
            // SHA512WITHECDSA

            Org.BouncyCastle.Crypto.ISignatureFactory signatureFactory = 
                new Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory("SHA512WITHRSA", keypair.Private, random)
            ;



            var gen = new Org.BouncyCastle.X509.X509V3CertificateGenerator();


            var CN = new Org.BouncyCastle.Asn1.X509.X509Name("CN=" + certName);
            var SN = Org.BouncyCastle.Math.BigInteger.ProbablePrime(120, new Random());

            gen.SetSerialNumber(SN);
            gen.SetSubjectDN(CN);
            gen.SetIssuerDN(CN);
            gen.SetNotAfter(DateTime.Now.AddYears(1));
            gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            gen.SetPublicKey(keypair.Public);


            // -- Are these necessary ? 

            // public static readonly DerObjectIdentifier AuthorityKeyIdentifier = new DerObjectIdentifier("2.5.29.35");
            // OID value: 2.5.29.35
            // OID description: id-ce-authorityKeyIdentifier
            // This extension may be used either as a certificate or CRL extension. 
            // It identifies the public key to be used to verify the signature on this certificate or CRL.
            // It enables distinct keys used by the same CA to be distinguished (e.g., as key updating occurs).

            
            // http://stackoverflow.com/questions/14930381/generating-x509-certificate-using-bouncy-castle-java
            gen.AddExtension(
            Org.BouncyCastle.Asn1.X509.X509Extensions.AuthorityKeyIdentifier.Id,
            false,
            new Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier(
                Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keypair.Public),
                new Org.BouncyCastle.Asn1.X509.GeneralNames(new Org.BouncyCastle.Asn1.X509.GeneralName(CN)),
                SN
            ));

            // OID value: 1.3.6.1.5.5.7.3.1
            // OID description: Indicates that a certificate can be used as an SSL server certificate.
            gen.AddExtension(
                Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id,
                false,
                new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(new ArrayList()
                {
                new Org.BouncyCastle.Asn1.DerObjectIdentifier("1.3.6.1.5.5.7.3.1")
            }));

            // -- End are these necessary ? 

            Org.BouncyCastle.X509.X509Certificate bouncyCert = gen.Generate(signatureFactory);

            byte[] ba = bouncyCert.GetEncoded();
            System.Security.Cryptography.X509Certificates.X509Certificate2 msCert = new System.Security.Cryptography.X509Certificates.X509Certificate2(ba);
            return msCert;
        }


        // https://stackoverflow.com/questions/6128541/bouncycastle-privatekey-to-x509certificate2-privatekey
        public static Org.BouncyCastle.X509.X509Certificate CreateX509Cert(string certName)
        {
            
            var keypairgen = new Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator();
            keypairgen.Init(new Org.BouncyCastle.Crypto.KeyGenerationParameters(
                new Org.BouncyCastle.Security.SecureRandom(
                    new Org.BouncyCastle.Crypto.Prng.CryptoApiRandomGenerator()
                    )
                    , 1024
                    )
            );

            var keypair = keypairgen.GenerateKeyPair();

            var gen = new Org.BouncyCastle.X509.X509V3CertificateGenerator();


            var CN = new Org.BouncyCastle.Asn1.X509.X509Name("CN=" + certName);
            var SN = Org.BouncyCastle.Math.BigInteger.ProbablePrime(120, new Random());

            gen.SetSerialNumber(SN);
            gen.SetSubjectDN(CN);
            gen.SetIssuerDN(CN);
            gen.SetNotAfter(DateTime.Now.AddYears(1));
            gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            gen.SetSignatureAlgorithm("MD5WithRSA");
            gen.SetPublicKey(keypair.Public);

            gen.AddExtension(
                Org.BouncyCastle.Asn1.X509.X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                new Org.BouncyCastle.Asn1.X509.AuthorityKeyIdentifier(
                    Org.BouncyCastle.X509.SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keypair.Public),
                    new Org.BouncyCastle.Asn1.X509.GeneralNames(new Org.BouncyCastle.Asn1.X509.GeneralName(CN)),
                    SN
                ));

            gen.AddExtension(
                Org.BouncyCastle.Asn1.X509.X509Extensions.ExtendedKeyUsage.Id,
                false,
                new Org.BouncyCastle.Asn1.X509.ExtendedKeyUsage(new ArrayList()
                {
                new Org.BouncyCastle.Asn1.DerObjectIdentifier("1.3.6.1.5.5.7.3.1")
                }));

            Org.BouncyCastle.X509.X509Certificate newCert = gen.Generate(keypair.Private);
            

            return newCert;
        }



        // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
        public static Org.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate2 x509Cert)
        {
            // https://stackoverflow.com/questions/8136651/how-can-i-convert-a-bouncycastle-x509certificate-to-an-x509certificate2
            // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
            return new Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(x509Cert.GetRawCertData());
        }

    }


    public static class NetExtension
    {

        // https://msdn.microsoft.com/en-us/library/hh873178(v=vs.110).aspx
        public static IAsyncResult AsApm(this Task task,
                                   AsyncCallback callback,
                                   object state)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<bool>(state);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else tcs.TrySetResult(true);

                if (callback != null)
                    callback(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        public static IAsyncResult AsApm<T>(this Task<T> task,
                                    AsyncCallback callback,
                                    object state)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                if (callback != null)
                    callback(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }


        public static TOut[] ConvertAll<TIn, TOut>(this TIn[] thisArray, Func<TIn, TOut> converter)
        {
            if (thisArray == null)
                throw new ArgumentNullException(nameof(thisArray));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            TOut[] revVal = new TOut[thisArray.Length];

            for (int i = 0; i < thisArray.Length; i++)
                revVal[i] = converter(thisArray[i]);

            return revVal;
        }


        // public static byte[] GetRawCertData(this Org.BouncyCastle.X509.X509Certificate cert)
        public static byte[] GetRawCertData(this System.Security.Cryptography.X509Certificates.X509Certificate2 cert)
        {
            // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = (System.Security.Cryptography.X509Certificates.X509Certificate2)cert;
            return cert2.RawData;
        }


        public static void Connect(this UdpClient udpClient, System.Net.IPAddress address, int port)
        {
            // Just ignore it, new in SendAsync
        }


        //public async static Task<int> SendAsync(this UdpClient udpClient, byte[] datagram, int bytes)
        //{
        //    return 0;
        //}


        public static IAsyncResult BeginConnect(this TcpClient tcpClient, System.Net.IPAddress address, int port, AsyncCallback requestCallback, object state)
        {
            return tcpClient.ConnectAsync(address, port).AsApm(requestCallback, state);
        }


        public static bool EndConnect(this TcpClient tcpClient, IAsyncResult asyncResult)
        {
            return ((Task<bool>)asyncResult).Result;
        }
        

        public static bool WaitOne(this System.Threading.WaitHandle wh, TimeSpan timeout, bool exitContext)
        {
            return wh.WaitOne(timeout);
        }


        public static void Close(this UdpClient udpClient)
        {
        }


        public static void Close(this TcpClient tcpClient)
        {
        }


        public static void Close(this System.Threading.WaitHandle handle)
        {
        }


    }


}
