using System;

namespace SWEN1.MTCG.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new HTTPServer();
            x.Start();
            Console.ReadLine();
            x.Stop();
        }
    }
}