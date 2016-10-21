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
            SimpleServer.Test();

            Console.WriteLine(System.Environment.NewLine);
            Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }
    }
}
