using System;
using System.Net;
using System.Threading;

namespace WebChat
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Thread(() =>
            {
                try
                {
                    DoWork(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            t.Start();
        }

        static void DoWork(string[] args)
        {
            if (args.Length == 1)
            {
                if (int.TryParse(args[0], out var port))
                {
                    var server = new Server(port);
                    server.Start();
                }
                else
                {

                }
            }
            else if (args.Length == 2)
            {
                if (IPAddress.TryParse(args[0], out var address) && int.TryParse(args[1], out var port))
                {
                    var client = new Client(address, port);
                    client.Start();
                }
                else
                {

                }
            }
            else
            {
                Console.WriteLine("Передайте в args либо порт, либо порт и адрес");
            }
        }
    }
}
