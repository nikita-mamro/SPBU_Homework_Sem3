using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
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
        
        public async Task<List<(string, bool)>> List(string path)
        {
            client = new TcpClient(server, port);

            using (var stream = client.GetStream())
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var reader = new StreamReader(stream);

                await writer.WriteLineAsync("1" + path);

                var response = await reader.ReadLineAsync();
                Console.WriteLine(response);

                var res = ResponseHandler.HandleListResponse(response);
                return res;
            }
        }

        public async Task Get(string pathFrom, string pathTo)
        {

        }

        public void Stop()
        {
            client.Close();
        }
    }
}
