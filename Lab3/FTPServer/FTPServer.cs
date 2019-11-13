using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FTPServer
{
    public class FTPServer
    {
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private int port;
        private TcpListener listener;
        private CancellationTokenSource cts;

        public FTPServer(int port)
        {
            this.port = port;
            listener = new TcpListener(localAddress, port);
            cts = new CancellationTokenSource();
        }

        public async Task Start()
        {
            try
            {
                listener.Start();

                Console.WriteLine("Сервер запущен");

                while (!cts.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        var client = await listener.AcceptTcpClientAsync();

                        while (!cts.IsCancellationRequested)
                        {
                            try
                            {
                                HandleConnection(client);
                            }
                            catch (Exception e)
                            {
                                client.Close();
                                Console.WriteLine("Вызвано исключение: " + e.Message);
                                break;
                            }
                        }

                        client.Close();
                    });
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
            cts.Cancel();

            if (listener != null)
            {
                listener.Stop();
            }
        }

        private void HandleConnection(TcpClient client)
        {
            Task.Run(async () =>
            {
                var stream = client.GetStream();

                var reader = new StreamReader(stream);
                
                var mes = await reader.ReadLineAsync();
                
                Console.WriteLine(mes);

                //var data = new byte[256];
                //
                //var bytes = await stream.ReadAsync(data, 0, data.Length);
                //
                //var request = Encoding.UTF8.GetString(data, 0, bytes);
                //
                //Console.WriteLine(request);
                //
                //byte[] response;
                ////byte[] responseSize;
                //
                //try
                //{
                //    response = Handlers.RequestHandler.HandleRequest(request.ToString());
                //    //responseSize = BitConverter.GetBytes(response.Length);
                //    //await stream.WriteAsync(responseSize, 0, responseSize.Length);
                //    await stream.WriteAsync(response, 0, response.Length);
                //}
                //catch (Exception e)
                //{
                //    var exceptionBytes = Encoding.UTF8.GetBytes(e.Message);
                //    //responseSize = BitConverter.GetBytes(exceptionBytes.Length);
                //    //await stream.WriteAsync(responseSize, 0, responseSize.Length);
                //    await stream.WriteAsync(exceptionBytes, 0, exceptionBytes.Length);
                //}
            });
        }
    }
}
