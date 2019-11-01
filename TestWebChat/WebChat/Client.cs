using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace WebChat
{
    public class Client
    {
        private TcpClient client;
        private int port;
        private IPAddress hostAddress;

        public Client(IPAddress address, int port)
        {
            this.port = port;
            hostAddress = address;

            client = new TcpClient();
        }

        private void Recieve(NetworkStream stream)
        {
            var data = new byte[256];
            var response = new StringBuilder();

            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable); // пока данные есть в потоке

            Console.WriteLine(response.ToString());
        }

        private void Send(NetworkStream stream)
        {
            // сообщение для отправки
            var response = Console.ReadLine();
            // преобразуем сообщение в массив байтов
            var data = Encoding.UTF8.GetBytes(response);

            // отправка сообщения
            stream.Write(data, 0, data.Length);
            //Console.WriteLine("Отправлено сообщение: {0}", response);

            
        }

        public void Start()
        {
            try
            {
                client.Connect(hostAddress, port);

                Console.WriteLine("Клиент подключён к указанному порту по указанному адресу");

                var stream = client.GetStream();

                var sendingThread = new Thread(() =>
                {
                    while (true)
                    {
                        Send(stream);
                    }
                });

                var recievingThread = new Thread(() =>
                {
                    while (true)
                    {
                        Recieve(stream);
                    }
                });

                sendingThread.Start();
                recievingThread.Start();

                // Закрываем потоки
                //stream.Close();
                //client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }
    }
}
