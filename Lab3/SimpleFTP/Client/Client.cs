using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPClient
{
    public class Client
    {
        private string server;
        private int port;
        private TcpClient client;

        public Client(string server, int port)
        {
            this.server = server;
            this.port = port;
        }

        public void Connect()
        {
            client = new TcpClient(server, port);
        }

        public async Task Start()
        {
            Connect();
            Console.WriteLine("Клиент подключён к серверу");

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "stop")
                {
                    Stop();
                    Console.WriteLine("Клиент отключён");
                    break;
                }

                List<(string, bool)> res;

                try
                {
                    res = await List(input);

                    foreach (var e in res)
                    {
                        Console.WriteLine(e);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        
        public async Task<List<(string, bool)>> List(string path)
        {
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("1" + path);

            var response = await reader.ReadLineAsync();

            return ResponseHandler.HandleListResponse(response);
        }

        public async Task Get(string pathFrom, string pathTo)
        {
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("2" + pathFrom);

            var response = await reader.ReadLineAsync();

            long fileSize;

            if (!long.TryParse(response, out fileSize))
            {
                throw new Exception(response);
            }

            if (fileSize == -1)
            {
                throw new FileNotFoundException();
            }

            
        }

        public void Stop()
        {
            client.GetStream().Dispose();
            client.Close();
        }
    }
}
