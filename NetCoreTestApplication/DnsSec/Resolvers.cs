
using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    public class Resolvers
    {


        // Get all addresses for a domain name using the local configured DNS servers
        public static void Test1()
        {
            IDnsResolver resolver = new DnsStubResolver();
            System.Collections.Generic.List<System.Net.IPAddress> addresses = resolver.ResolveHost("www.example.com");
        } // End Sub Test1 


        // Get reverse domain name for a specified IP address using local configured DNS servers
        public static void Test2()
        {
            IDnsResolver resolver = new DnsStubResolver();
            ARSoft.Tools.Net.DomainName name = resolver.ResolvePtr(System.Net.IPAddress.Parse("192.0.2.1"));
        } // End Sub Test2 


        // Get all MX records for a domain using a recursive resolver
        public static void Test3()
        {
            IDnsResolver resolver = new RecursiveDnsResolver();
            System.Collections.Generic.List<MxRecord> mxRecords = resolver.Resolve<MxRecord>("example.com", RecordType.Mx);
        } // End Sub Test3 


        // Get all MX records for a domain using a recursive resolver
        public static void Test4()
        {
            System.Net.NetworkInformation.IPGlobalProperties ipgp =
                System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            // IDnsResolver resolver = new RecursiveDnsResolver(); // Warning: Doesn't work
            IDnsResolver resolver = new DnsStubResolver();
            System.Collections.Generic.List<SrvRecord> srvRecords = resolver.Resolve<SrvRecord>("_ldap._tcp." + ipgp.DomainName, RecordType.Srv);

            foreach (SrvRecord thisRecord in srvRecords)
            {
                // System.Console.WriteLine(thisRecord.Name);
                System.Console.WriteLine(thisRecord.Target);
                System.Console.WriteLine(thisRecord.Port);

                string url = "LDAP://" + thisRecord.Target + ":" + thisRecord.Port; // Note: OR LDAPS:// - but Novell doesn't want these parts anyway 
                System.Console.WriteLine(url);
            } // Next thisRecord

        } // End Sub Test4 


    } // End Class Resolvers 


} // End Namespace ArsoftTestServer 
