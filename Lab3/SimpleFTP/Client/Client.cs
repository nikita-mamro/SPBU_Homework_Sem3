using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
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

        public async Task Start()
        {
            try
            {
                client = new TcpClient(server, port);

                Console.WriteLine("Клиент подключён к серверу");

                while (client.Connected)
                {
                    try
                    {
                        var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                        var reader = new StreamReader(client.GetStream());
                        
                        
                        var path = Console.ReadLine();
                        await writer.WriteLineAsync("1" + path);
                        var response = await reader.ReadLineAsync();
                        Console.WriteLine(response); //works
                        
                        //var res = ResponseHandler.HandleListResponse(response);

                        var res = await List(path); // crashes
                        //
                        //foreach (var e in res)
                        //{
                        //    Console.WriteLine(e.Item1);
                        //}
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Console.WriteLine("Сервер разорвал поделючение");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Not working
        private async Task<List<(string, bool)>> List(string path)
        {
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteAsync("1" + path);
            var response = await reader.ReadLineAsync();

            Console.WriteLine(response);

            return ResponseHandler.HandleListResponse(response);
        }


        public void Stop()
        {
            client.Close();
        }
    }
}
