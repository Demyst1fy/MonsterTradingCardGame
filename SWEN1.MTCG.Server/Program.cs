using System;
using System.Text;

namespace ConsoleApp1
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