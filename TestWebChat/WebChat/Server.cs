using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebChat
{
    public class Server
    {
        private TcpListener listener;
        private int port;
        private IPAddress address = IPAddress.Parse("127.0.0.1");
        private volatile bool isAlive = true;

        public Server(int port)
        {
            this.port = port;

            listener = new TcpListener(address, port);
        }

        private void Recieve(NetworkStream stream)
        {
            var data = new byte[256];
            var message = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                message.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable); // пока данные есть в потоке

            if (message.ToString() == "exit")
            {
                isAlive = false;
            }

            Console.WriteLine(message.ToString());
        }

        private void Send(NetworkStream stream)
        {
            var response = Console.ReadLine();

            var data = Encoding.UTF8.GetBytes(response);
            
            stream.Write(data, 0, data.Length);
        }

        public void Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Сервер запущен");

                while (true)
                {
                    // получаем входящее подключение
                    var client = listener.AcceptTcpClient();

                    // получаем сетевой поток для чтения и записи
                    var stream = client.GetStream();

                    var sendingThread = new Thread(() =>
                    {
                        while (isAlive)
                        {
                            Send(stream);
                        }

                        stream.Close();
                        listener.Stop();
                    });

                    var recievingThread = new Thread(() =>
                    {
                        while (isAlive)
                        {
                            Recieve(stream);
                        }

                        stream.Close();
                        listener.Stop();
                    });

                    sendingThread.Start();
                    recievingThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                
            }
        }
    }
}
