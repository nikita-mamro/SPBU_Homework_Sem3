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
        private volatile bool isAlive = true;
        private object sendLocker = new object();
        private object recieveLocker = new object();

        public Client(IPAddress address, int port)
        {
            this.port = port;
            hostAddress = address;

            client = new TcpClient();
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
            while (stream.DataAvailable);

            Console.WriteLine(message.ToString());
        }

        private void Send(NetworkStream stream)
        {
            var message = Console.ReadLine();

            var data = Encoding.UTF8.GetBytes(message);

            stream.Write(data, 0, data.Length);

            if (message.ToString() == "exit")
            {
                lock (sendLocker)
                {
                    lock (recieveLocker)
                    {
                        isAlive = false;
                    }
                }
            }
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
                    while (isAlive)
                    {
                        Send(stream);
                    }

                    lock (sendLocker)
                    {
                        stream.Close();
                        client.Close();
                    }
                });

                var recievingThread = new Thread(() =>
                {
                    while (isAlive)
                    {
                        Recieve(stream);
                    }

                    lock (recieveLocker)
                    {
                        stream.Close();
                        client.Close();
                    }
                });

                sendingThread.Start();
                recievingThread.Start();
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
