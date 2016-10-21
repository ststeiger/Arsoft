using System;
using System.Diagnostics;

using System.Threading;
using System.Globalization;

using Heijden.DNS;

namespace DnsDig
{
	class Dig
	{
		public Resolver resolver;

		public Dig()
		{
			resolver = new Resolver();
			resolver.OnVerbose += new Resolver.VerboseEventHandler(resolver_OnVerbose);
		}

		private void resolver_OnVerbose(object sender, Resolver.VerboseEventArgs e)
		{
			Console.WriteLine(e.Message);
		}

		public void DigIt(string name)
		{
			DigIt(name, QType.A, QClass.IN);
		}

		public void DigIt(string name, QType qtype)
		{
			DigIt(name, qtype, QClass.IN);
		}

		private delegate void DigItDelegate(string name, QType qtype, QClass qclass);
		public void BeginDigIt(string name, QType qtype, QClass qclass)
		{
			DigItDelegate d = new DigItDelegate(DigIt);
			d.BeginInvoke(name, qtype, qclass,null,null);
		}

		public void DigIt(string name, QType qtype, QClass qclass)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);

			Console.WriteLine("; <<>> Dig.Net {0} <<>> @{1} {2} {3}", resolver.Version, resolver.DnsServer, qtype, name);
			Console.WriteLine(";; global options: printcmd");

			Stopwatch sw = new Stopwatch();

			sw.Start();
			Response response = resolver.Query(name, qtype, qclass);
			sw.Stop();

			if(response.Error != "")
			{
				Console.WriteLine(";; " + response.Error);
				return;
			}

			Console.WriteLine(";; Got answer:");

			Console.WriteLine(";; ->>HEADER<<- opcode: {0}, status: {1}, id: {2}",
				response.header.OPCODE,
				response.header.RCODE,
				response.header.ID);
			Console.WriteLine(";; flags: {0}{1}{2}{3}; QUERY: {4}, ANSWER: {5}, AUTHORITY: {6}, ADDITIONAL: {7}",
				response.header.QR ? " qr" : "",
				response.header.AA ? " aa" : "",
				response.header.RD ? " rd" : "",
				response.header.RA ? " ra" : "",
				response.header.QDCOUNT,
				response.header.ANCOUNT,
				response.header.NSCOUNT,
				response.header.ARCOUNT);
			Console.WriteLine("");

			if (response.header.QDCOUNT > 0)
			{
				Console.WriteLine(";; QUESTION SECTION:");
				foreach (Question question in response.Questions)
					Console.WriteLine(";{0}" , question);
				Console.WriteLine("");
			}

			if (response.header.ANCOUNT > 0)
			{
				Console.WriteLine(";; ANSWER SECTION:");
				foreach (AnswerRR answerRR in response.Answers)
					Console.WriteLine(answerRR);
				Console.WriteLine("");
			}

			if (response.header.NSCOUNT > 0)
			{
				Console.WriteLine(";; AUTHORITY SECTION:");
				foreach (AuthorityRR authorityRR in response.Authorities)
					Console.WriteLine(authorityRR);
				Console.WriteLine("");
			}

			if (response.header.ARCOUNT > 0)
			{
				Console.WriteLine(";; ADDITIONAL SECTION:");
				foreach (AdditionalRR additionalRR in response.Additionals)
					Console.WriteLine(additionalRR);
				Console.WriteLine("");
			}

			Console.WriteLine(";; Query time: {0} msec", sw.ElapsedMilliseconds);
			Console.WriteLine(";; SERVER: {0}#{1}({2})" ,response.Server.Address,response.Server.Port,response.Server.Address);
			Console.WriteLine(";; WHEN: " + response.TimeStamp.ToString("ddd MMM dd HH:mm:ss yyyy",new System.Globalization.CultureInfo("en-US")));
			Console.WriteLine(";; MSG SIZE rcvd: " + response.MessageSize);
		}
	}
}
