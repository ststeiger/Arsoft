#if WITH_UNIT_TESTS

using System;
using System.Net;
using System.Diagnostics;

using Heijden.DNS;

using NUnit.Framework;

// Use: C:\Program Files\NUnit\bin\nunit.exe
// Open: dnsdig.exe
//
namespace DnsDig
{
    [TestFixture]
    public class NUnitTester
    {
        Resolver resolver;

        [SetUp]
        public void Init()
        {
            resolver = new Resolver();
        }

        [Test]
        public void Test1()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();

            // The .NET DNS class uses cached data
            // try > ipconfig /flushdns
            // this clears the cache, and shows real resolving
            sw1.Start();
            IPHostEntry e1 = Dns.GetHostEntry("www.microsoft.com");
            sw1.Stop();

            Console.WriteLine("Dns.GetHostEntry " + sw1.ElapsedMilliseconds + " mSec");

            sw2.Start();
            IPHostEntry e2 = resolver.GetHostEntry("www.microsoft.com");
            sw2.Stop();

            Console.WriteLine("resolver.GetHostEntry " + sw2.ElapsedMilliseconds + " mSec");

            sw3.Start();
            IPHostEntry e3 = resolver.GetHostEntry("www.microsoft.com");
            sw3.Stop();

            Console.WriteLine("resolver.GetHostEntry " + sw3.ElapsedMilliseconds + " mSec");
        }

        [Test]
        public void Test2()
        {
            Response response = resolver.Query("www.microsoft.com", QType.A);
            foreach (RecordA recordA in response.RecordsA)
            {
                Console.WriteLine("RecordA " + recordA.Address);

                // every record has a reference to its Resource Record
                Console.WriteLine("RR TTL " + recordA.RR.TTL);
            }

            // Asynd testing
            IAsyncResult ar = resolver.BeginGetHostEntry("www.microsoft.com", null, null);

            bool blnResult = ar.AsyncWaitHandle.WaitOne(5000, false);

            Console.WriteLine("Result {0}", blnResult);

            IPHostEntry iPHostEntry = resolver.EndGetHostEntry(ar);

            Console.WriteLine("EndGetHostEntry (via HostName) returns {0}", iPHostEntry.HostName);

            ar = resolver.BeginGetHostEntry(IPAddress.Parse("207.46.193.254"), null, null);

            blnResult = ar.AsyncWaitHandle.WaitOne(5000, false);

            Console.WriteLine("Result {0}", blnResult);

            iPHostEntry = resolver.EndGetHostEntry(ar);

            Console.WriteLine("EndGetHostEntry (via IP) returns {0}", iPHostEntry.HostName);

        }


        [Test]
        public void Test3()
        {
            Response resp1 = resolver.Query("microsoft.com", QType.MX);
            foreach (RecordMX recordMX in resp1.RecordsMX)
            {
                Console.WriteLine("RecordMX " + recordMX);
                Response resp2 = resolver.Query(recordMX.EXCHANGE, QType.A);
                foreach (RecordA recordA in resp2.RecordsA)
                {
                    Console.WriteLine("->" + recordA);
                }
            }
        }

        [Test]
        public void TestGetArpaFromIp()
        {
            string s = "4321:0:1:2:3:4:567:89ab";

            string a = Resolver.GetArpaFromIp(IPAddress.Parse(s));

            Assert.AreEqual(a, "b.a.9.8.7.6.5.0.4.0.0.0.3.0.0.0.2.0.0.0.1.0.0.0.0.0.0.0.1.2.3.4.ip6.arpa.");
        }

        [Test]
        public void TestNaptr()
        {
            string TelephoneNumber = "+1 800-555-5555";
            //string TelephoneNumber = "+1 567-459-0088";
            Resolver resolver1 = new Resolver();
            resolver1.DnsServer = "E164.org";
            string strEnum = Resolver.GetArpaFromEnum(TelephoneNumber);

            Assert.AreEqual(strEnum, "5.5.5.5.5.5.5.0.0.8.1.e164.arpa.");
            //Assert.AreEqual(strEnum, "8.8.0.0.9.5.4.7.6.5.1.e164.arpa.");

            Response response = resolver1.Query(strEnum, QType.NAPTR, QClass.IN);
            foreach (RR rr in response.Answers)
                Console.WriteLine(rr.ToString());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetHostByAddress1()
        {
            string s = null;
            resolver.GetHostByAddress(s);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestGetHostByAddress2()
        {
            IPAddress i = null;
            resolver.GetHostByAddress(i);
        }

    }
}
#endif
