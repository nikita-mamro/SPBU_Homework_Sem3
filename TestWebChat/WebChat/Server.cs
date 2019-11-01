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

        public Server(int port)
        {
            this.port = port;

            listener = new TcpListener(address, port);
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
            // сообщение для отправки клиенту
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
                listener.Start();
                Console.WriteLine("Сервер запущен");

                while (true)
                {
                    // получаем входящее подключение
                    var client = listener.AcceptTcpClient();
                    //Console.WriteLine("Подключен клиент. Выполнение запроса...");

                    // получаем сетевой поток для чтения и записи
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
