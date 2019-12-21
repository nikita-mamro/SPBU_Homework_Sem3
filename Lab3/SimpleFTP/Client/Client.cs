using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FTPClient
{
    /// <summary>
    /// Класс, реализующий FTP клиента
    /// </summary>
    public class Client : IDisposable
    {
        /// <summary>
        /// Адрес сервера, к которому должен подключаться клиент
        /// </summary>
        private string server;

        /// <summary>
        /// Номер порта для подключения
        /// </summary>
        private int port;

        /// <summary>
        /// Обеспечивает подключение к серверу
        /// </summary>
        private TcpClient client;

        public Client(string server, int port)
        {
            this.server = server;
            this.port = port;
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public void Connect()
        {
            client = new TcpClient(server, port);
        }

        /// <summary>
        /// Для демонстрации работы с запросом List
        /// </summary>
        public async Task Start()
        {
            Connect();
            Console.WriteLine("Клиент подключён к серверу");

            while (true)
            {
                var input = Console.ReadLine();

                if (input == "stop")
                {
                    Stop();
                    Console.WriteLine("Клиент отключён");
                    break;
                }

                try
                {
                    var res = await List(input);

                    foreach (var e in res)
                    {
                        Console.WriteLine(e);
                    }
                }
                catch (Exception e) when (
                    e is SocketException
                    || e is IOException
                    )
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Запрос на получение списка файлов и папок по указанному пути на сервере
        /// </summary>
        /// <returns>Список значений (имя, папка - true / файл - false) </returns>
        public async Task<List<(string, bool)>> List(string path)
        {
            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("1" + path);

            var response = await reader.ReadLineAsync();

            return ResponseHandler.HandleListResponse(response);
        }

        /// <summary>
        /// Скачивание файла с сервера
        /// </summary>
        /// <param name="pathFrom">Путь к файлу на сервере</param>
        /// <param name="pathTo">Путь к месту скачивания</param>
        public async Task Get(string pathFrom, string pathTo)
        {
            var tmp = pathFrom.Split('\\');
            var fileName = tmp[tmp.Length - 1];

            var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
            var reader = new StreamReader(client.GetStream());

            await writer.WriteLineAsync("2" + pathFrom);

            var response = await reader.ReadLineAsync();

            if (!long.TryParse(response, out long fileSize))
            {
                throw new Exception(response);
            }

            if (fileSize == -1)
            {
                throw new FileNotFoundException();
            }

            using (var fileStream = new FileStream(pathTo + fileName, FileMode.CreateNew))
            {
                await reader.BaseStream.CopyToAsync(fileStream);
            }
        }

        /// <summary>
        /// Остановка работы клиента
        /// </summary>
        public void Stop()
        {
            client.Close();
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
