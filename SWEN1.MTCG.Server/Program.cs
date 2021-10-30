using System;
using SWEN1.MTCG.Server.Interfaces;

namespace SWEN1.MTCG.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            IHttpServer x = new HttpServer();
            x.Start(10001);
            Console.WriteLine("Welcome to the MTCG-Server. Waiting for requests...");
            Console.ReadLine();
            x.Stop();
        }
    }
}