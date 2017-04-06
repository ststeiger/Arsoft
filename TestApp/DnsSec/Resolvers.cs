using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ArsoftTestServer
{
    public class Resolvers
    {


        // Get all addresses for a domain name using the local configured DNS servers
        public static void Test1()
        {
            IDnsResolver resolver = new DnsStubResolver();
            List<IPAddress> addresses = resolver.ResolveHost("www.example.com");
        }


        // Get reverse domain name for a specified IP address using local configured DNS servers
        public static void Test2()
        {
            IDnsResolver resolver = new DnsStubResolver();
            DomainName name = resolver.ResolvePtr(IPAddress.Parse("192.0.2.1"));
        }


        // Get all MX records for a domain using a recursive resolver
        public static void Test3()
        {
            IDnsResolver resolver = new RecursiveDnsResolver();
            List<MxRecord> mxRecords = resolver.Resolve<MxRecord>("example.com", RecordType.Mx);
        }


        // Get all MX records for a domain using a recursive resolver
        public static void Test4()
        {
            System.Net.NetworkInformation.IPGlobalProperties ipgp = 
                System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            
            // IDnsResolver resolver = new RecursiveDnsResolver(); // Warning: Doesn't work
            IDnsResolver resolver = new DnsStubResolver();
            List<SrvRecord> srvRecords = resolver.Resolve<SrvRecord>("_ldap._tcp." + ipgp.DomainName, RecordType.Srv);

            foreach (SrvRecord thisRecord in srvRecords)
            {
                // System.Console.WriteLine(thisRecord.Name);
                System.Console.WriteLine(thisRecord.Target);
                System.Console.WriteLine(thisRecord.Port);

                string url = "LDAP://" + thisRecord.Target + ":" + thisRecord.Port; // Note: OR LDAPS:// - but Novell doesn't want these parts anyway 
                System.Console.WriteLine(url);
            } // Next thisRecord

        }


    }
}
