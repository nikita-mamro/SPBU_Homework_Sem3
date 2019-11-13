using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private int port;
        private TcpListener listener;

        public Server(int port)
        {
            this.port = port;
            listener = new TcpListener(localAddress, port);
        }

        public async Task Start()
        {
            try
            {
                listener.Start();

                Console.WriteLine("Сервер запущен");

                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();

                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                    while (true)
                    {
                        var recieved = await reader.ReadLineAsync();
                        Console.WriteLine(recieved);
                        await writer.WriteLineAsync(RequestHandler.HandleRequest(recieved));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Stop();
            }
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
    }
}
