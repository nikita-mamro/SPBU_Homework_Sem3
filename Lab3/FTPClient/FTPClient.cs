using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace FTPClient
{
    public class FTPClient
    {
        private string server;
        private int port;
        private TcpClient client;
        private CancellationTokenSource cts;

        public FTPClient(string server, int port)
        {
            this.server = server;
            this.port = port;
            cts = new CancellationTokenSource();
        }

        public async Task Start()
        {
            try
            {
                client = new TcpClient(server, port);

                Console.WriteLine("Клиент подключён к серверу");

                while (client.Connected)
                {
                    var path = Console.ReadLine();

                    try
                    {
                        //var stream = client.GetStream();
                        //
                        //var request = Encoding.UTF8.GetBytes("1" + path);
                        //await stream.WriteAsync(request, 0, request.Length);
                        //
                        //var buffer = new byte[256];
                        //
                        //var responseBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                        //
                        //var response = Encoding.UTF8.GetString(buffer, 0, responseBytes);
                        //
                        //Console.WriteLine(response);

                        var writer = new StreamWriter(client.GetStream());

                        await writer.WriteAsync(path);
                        
                        //foreach (var e in res)
                        //{
                        //    Console.WriteLine(e);
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
                cts.Cancel();
            }
        }

        public void Stop()
        {
            cts.Cancel();
            client.Close();
        }

        public async Task<List<(string, bool)>> List(string path)
        {
            try
            {
                var client = new TcpClient(server, port);

                var stream = client.GetStream();
                {
                    //var request = Encoding.UTF8.GetBytes("1" + path);
                    //await stream.WriteAsync(request, 0, request.Length);
                    //
                    //var buffer = new byte[256];

                    //var responseSizeBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    //
                    //var responseSize = BitConverter.ToInt64(buffer, 0);
                    //
                    //buffer = new byte[responseSize];
                    //var responseBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                    //
                    //Console.WriteLine("test2");
                    //
                    //var response = Encoding.UTF8.GetString(buffer, 0, responseBytes);
                    //
                    //Console.WriteLine(response);

                    //return ResponseHandler.HandleListResponse(response);

                    return null;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public async Task Get(string path)
        {

        }

        

        //private async Task<string> ProceedRequest(string request, NetworkStream stream)
        //{
        //    var reader = new StreamReader(stream);
        //    var writer = new StreamWriter(stream);
        //
        //    await writer.WriteLineAsync(request);
        //    var response = await reader.ReadLineAsync();
        //
        //    return response;
        //}
    }
}
