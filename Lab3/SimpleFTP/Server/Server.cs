﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FTPServer
{
    /// <summary>
    /// Класс, реализующий FTP сервер
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Адрес сервера
        /// </summary>
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        /// <summary>
        /// Порт, по которому должны подключаться клиенты
        /// </summary>
        private int port;
        /// <summary>
        /// Обеспечивает прослушивание подключений
        /// </summary>
        private TcpListener listener;
        /// <summary>
        /// Отвечает за отмену процессов после выключения сервера
        /// </summary>
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

                while (!cts.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync();

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
                while (!cts.IsCancellationRequested)
                {
                    if (!IsConnected(client))
                    {
                        break;
                    }

                    var reader = new StreamReader(client.GetStream());
                    var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                    var recieved = await reader.ReadLineAsync();

                    await RequestHandler.HandleRequest(recieved, writer);
                }
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
        }
    }
}
