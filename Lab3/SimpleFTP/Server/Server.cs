using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{
    public class Server
    {
        /// <summary>
        /// Необходимые для взаимодействия с клиентами объекты
        /// </summary>
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private int port;
        private TcpListener listener;
        private CancellationTokenSource cts;

        public Server(int port)
        {
            cts = new CancellationTokenSource();
            this.port = port;
            listener = new TcpListener(localAddress, port);
        }

        /// <summary>
        /// Начало работы сервера
        /// </summary>
        public async Task Start()
        {
            try
            {
                listener.Start();

                Console.WriteLine("Сервер запущен");

                while (!cts.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();

                    Console.WriteLine("Подключён новый клиент");

                    HandleClient(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Stop();
            }
        }

        /// <summary>
        /// Метод, ппроверяющий, подключён ли данный клиент к серверу
        /// </summary>
        private bool IsConnected(TcpClient client)
        {
            try
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        var buffer = new byte[1];
                        if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
                        {
                            return false;
                        }
                    }

                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обработчик запросов, поступающих от клиента
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (!IsConnected(client))
                    {
                        break;
                    }

                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                    var recieved = await reader.ReadLineAsync();
                    Console.WriteLine(recieved);
                    
                    await RequestHandler.HandleRequest(recieved, writer);
                }

                Console.WriteLine("Работа с клиентом закончена");
            });
        }

        /// <summary>
        /// Остановка работы сервера
        /// </summary>
        public void Stop()
        {
            cts.Cancel();

            if (listener != null)
            {
                listener.Stop();
            }

            Console.WriteLine("Server stopped");
        }
    }
}
