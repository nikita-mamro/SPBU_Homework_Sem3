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

                //Console.WriteLine("Клиент подключен");

                using (var stream = client.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    var reader = new StreamReader(stream);

                    var path = Console.ReadLine();
                    await writer.WriteLineAsync("1" + path);

                    var response = await reader.ReadLineAsync();
                    Console.WriteLine(response); //works

                    var res = ResponseHandler.HandleListResponse(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Not working
        public async Task<List<(string, bool)>> List(string path)
        {
            var client = new TcpClient(server, port);

            using (var stream = client.GetStream())
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var reader = new StreamReader(stream);

                await writer.WriteAsync("1" + path);
                var response = await reader.ReadLineAsync();

                return ResponseHandler.HandleListResponse(response);
            }
        }


        public void Stop()
        {
            client.Close();
        }
    }
}
