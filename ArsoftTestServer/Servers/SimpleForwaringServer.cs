
using System;
using System.Net;
using System.Threading.Tasks;

using ARSoft.Tools.Net.Dns;


namespace ArsoftTestServer
{


    // https://docs.ar-soft.de/arsoft.tools.net/DNS%20Server.html
    class SimpleForwaringServer
    {
        public static void Test()
        {
            using (DnsServer server = new DnsServer(IPAddress.Any, 10, 10))
            {
                server.QueryReceived += OnQueryReceived;

                server.Start();

                Console.WriteLine("Press any key to stop server");
                Console.ReadLine();
            } // End Using server 

        } // End Sub Test 


        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e)
        {
            DnsMessage message = e.Query as DnsMessage;

            if (message == null)
                return;

            DnsMessage response = message.CreateResponseInstance();

            if ((message.Questions.Count == 1))
            {
                // send query to upstream server
                DnsQuestion question = message.Questions[0];
                DnsMessage upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);

                // if got an answer, copy it to the message sent to the client
                if (upstreamResponse != null)
                {
                    foreach (DnsRecordBase record in (upstreamResponse.AnswerRecords))
                    {
                        response.AnswerRecords.Add(record);
                    } // Next record 

                    foreach (DnsRecordBase record in (upstreamResponse.AdditionalRecords))
                    {
                        response.AdditionalRecords.Add(record);
                    } // Next record 

                    response.ReturnCode = ReturnCode.NoError;

                    // set the response
                    e.Response = response;
                } // End if (upstreamResponse != null) 

            } // End if ((message.Questions.Count == 1)) 

        } // End Function OnQueryReceived 


    } // End Class SimpleForwaringServer 


} // End Namespace ArsoftTestServer 
