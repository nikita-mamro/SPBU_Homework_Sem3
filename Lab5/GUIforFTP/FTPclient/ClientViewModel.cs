using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FTPClient;

namespace ViewModel
{
    /// <summary>
    /// Класс, реализующий модель для GUI
    /// </summary>
    public class ClientViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Порт для подключения
        /// </summary>
        private int port;

        /// <summary>
        /// Адрес для подключения
        /// </summary>
        private string server;

        /// <summary>
        /// Клиент
        /// </summary>
        private Client client;

        /// <summary>
        /// Индикатор состояния подключения
        /// </summary>
        public bool IsConnected = false;

        /// <summary>
        /// Корневая папка клиента
        /// </summary>
        public string RootClientDirectoryPath;

        /// <summary>
        /// Путь до текущей папки в клиенте
        /// </summary>
        private string currentDirectoryOnClientPath;

        /// <summary>
        /// Путь до текушей папки на сервере
        /// </summary>
        private string сurrentDirectoryOnServer;

        /// <summary>
        /// Текущая папка в клиенте
        /// </summary>
        private string currentDirectoryOnClient;

        /// <summary>
        /// Полный путь для загрузки
        /// </summary>
        private string downloadPath;

        /// <summary>
        /// Папка для загрузок
        /// </summary>
        public string DownloadFolder
        {
            get
            {
                var tmp = downloadPath.Remove(downloadPath.Length - 1);
                return tmp.Substring(tmp.LastIndexOf('\\') + 1);
            }
            set
            {
                downloadPath = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Полные пути до текущих директорий на сервере
        /// </summary>
        private ObservableCollection<(string, bool)> currentPathsOnServer;

        /// <summary>
        /// Полные пути до текущих директорий в клиенте
        /// </summary>
        private ObservableCollection<string> currentPathsOnClient;

        /// <summary>
        /// Элементы обозревателя директории на сервере
        /// </summary>
        public ObservableCollection<string> DisplayedListOnServer { get; private set; }

        /// <summary>
        /// Элементы обозревателя папок в клиенте
        /// </summary>
        public ObservableCollection<string> DisplayedListOnClient { get; private set; }

        /// <summary>
        /// Список текущих загрузок
        /// </summary>
        public ObservableCollection<string> DownloadsInProgressList { get; private set; }

        /// <summary>
        /// Список завершенных загрузок
        /// </summary>
        public ObservableCollection<string> DownloadsFinishedList { get; private set; }


        /// <summary>
        /// Обработка ошибок
        /// </summary>
        public delegate void ShowErrorMessage(object sender, string message);

        public event ShowErrorMessage ThrowError = (_, __) => { };

        /// <summary>
        /// Обработчик изменения текущей папки для загрузки
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Порт для подключения
        /// </summary>
        public string Port
        {
            get
            {
                IsConnected = false;
                return port.ToString();
            }
            set
            {
                IsConnected = false;
                port = Convert.ToInt32(value);
            }
        }

        /// <summary>
        /// Адрес для подключения
        /// </summary>
        public string Address
        {
            get
                => server;
            set
                => server = value;
        }

        /// <summary>
        /// Инициализация модели
        /// </summary>
        public ClientViewModel(string rootClientDirectory)
        {
            server = "127.0.0.1";
            port = 8888;
            RootClientDirectoryPath = rootClientDirectory;
            сurrentDirectoryOnServer = "";

            currentDirectoryOnClientPath = RootClientDirectoryPath;
            currentDirectoryOnClient = "";
            downloadPath = RootClientDirectoryPath;

            DisplayedListOnServer = new ObservableCollection<string>();
            DisplayedListOnClient = new ObservableCollection<string>();
            
            DownloadsInProgressList = new ObservableCollection<string>();
            DownloadsFinishedList = new ObservableCollection<string>();

            InitializeCurrentPathsOnClient();
        }

        /// <summary>
        /// Уведомляет элемент интерфейса, отображающий текущую папку для загрузки, о её изменении
        /// </summary>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Подключение клиента к серверу
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }

            client = new Client(server, port);

            DisplayedListOnServer.Clear();

            try
            {
                client.Connect();
                await InitializeCurrentPathsOnServer();
                IsConnected = true;
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Инициализация текущего состояния путей к папкам в клиенте
        /// </summary>
        private void InitializeCurrentPathsOnClient()
        {
            currentPathsOnClient = new ObservableCollection<string>();

            currentPathsOnClient.CollectionChanged += CurrentPathsOnClientChanged;

            try
            {
                TryUpdateCurrentPathsOnClient("");
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Инициализация текущего состояние путей к файлам и папкам на сервере
        /// </summary>
        private async Task InitializeCurrentPathsOnServer()
        {
            currentPathsOnServer = new ObservableCollection<(string, bool)>();

            currentPathsOnServer.CollectionChanged += CurrentPathsOnServerChanged;
            
            await TryUpdateCurrentPathsOnServer("");
        }

        /// <summary>
        /// Обработчик изменения текущего состояния путей к папкам в клиенте
        /// </summary>
        private void CurrentPathsOnClientChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                   DisplayedListOnClient.Remove(item.ToString());
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    DisplayedListOnClient.Add(item.ToString());
                }
            }
        }

        /// <summary>
        /// Обработчик изменения текущего состояния путей к папка на сервере
        /// </summary>
        private void CurrentPathsOnServerChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach ((string, bool) pair in e.OldItems)
                {
                    DisplayedListOnServer.Remove(pair.Item1);
                }
            }

            if (e.NewItems != null)
            {
                foreach ((string, bool) pair in e.NewItems)
                {
                    DisplayedListOnServer.Add(pair.Item1);
                }
            }
        }

        /// <summary>
        /// Открытие папки в обозревателе клиента
        /// </summary>
        public void OpenClientFolder(string folderName)
        {
            var nextDirectoryPath = Path.Combine(currentDirectoryOnClientPath, folderName);

            if (Directory.Exists(nextDirectoryPath))
            {
                var nextDirectoryOnClient = Path.Combine(currentDirectoryOnClient, folderName);
                TryUpdateCurrentPathsOnClient(nextDirectoryOnClient);
                currentDirectoryOnClientPath = nextDirectoryPath;
                currentDirectoryOnClient = Path.Combine(currentDirectoryOnClient, folderName);
            }
            else
            {
                ThrowError(this, "Directory not found");
            }
        }

        /// <summary>
        /// Проверяет, является ли файлом элемент с данным именем
        /// </summary>
        private bool IsFile(string folderName)
        {
            foreach (var path in currentPathsOnServer)
            {
                if (path.Item1 == folderName)
                {
                    return !path.Item2;
                }
            }

            return false;
        }

        /// <summary>
        /// Открывает папку или скачивает файл по нажатии на элемент ListView в GUI
        /// </summary>
        public async Task OpenServerFolderOrDownloadFile(string folderName)
        {
            if (IsFile(folderName))
            {
                await DownloadFile(folderName);
                return;
            }

            var nextDirectory = Path.Combine(сurrentDirectoryOnServer, folderName);

            await TryUpdateCurrentPathsOnServer(nextDirectory);
        }

        /// <summary>
        /// Обновление путей в клиенте
        /// </summary>
        /// <param name="folderPath">Открытая папка</param>
        private void TryUpdateCurrentPathsOnClient(string folderPath)
        {
            try
            {
                var dirToOpen = Path.Combine(RootClientDirectoryPath, folderPath);

                var folders = Directory.EnumerateDirectories(dirToOpen);

                while (currentPathsOnClient.Count > 0)
                {
                    currentPathsOnClient.RemoveAt(currentPathsOnClient.Count - 1);
                }

                foreach (var folder in folders)
                {
                    var name = folder.Substring(folder.LastIndexOf('\\') + 1);
                    currentPathsOnClient.Add(name);
                }
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Обновление текущих путей на сервере
        /// </summary>
        /// <param name="listRequest">Открытая папка</param>
        private async Task TryUpdateCurrentPathsOnServer(string listRequest)
        {
            try
            {
                var serverList = await client.List(listRequest);

                while (currentPathsOnServer.Count > 0)
                {
                    currentPathsOnServer.RemoveAt(currentPathsOnServer.Count - 1);
                }

                foreach (var path in serverList)
                {
                    var name = path.Item1;

                    name = name.Substring(name.LastIndexOf('\\') + 1);

                    currentPathsOnServer.Add((name, path.Item2));
                }

                сurrentDirectoryOnServer = listRequest;
            }
            catch (Exception e)
            {
                if (e.Message == "-1")
                {
                    ThrowError(this, "Directory not found exception occured");
                    return;
                }

                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Возвращение назад в клиенте
        /// </summary>
        public void GoBackClient()
        {
            if (currentDirectoryOnClient == "")
            {
                ThrowError(this, "Can't go back from the root directory");
                return;
            }

            try
            {
                var index = currentDirectoryOnClient.LastIndexOf('\\');
                string toOpen;

                if (index > 0)
                {
                    toOpen = currentDirectoryOnClient.Substring(0, currentDirectoryOnClient.LastIndexOf('\\'));
                }
                else
                {
                    toOpen = "";
                }

                TryUpdateCurrentPathsOnClient(toOpen);
                currentDirectoryOnClient = toOpen;
                currentDirectoryOnClientPath = Directory.GetParent(currentDirectoryOnClientPath).ToString();
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Возвращение назад на сервере
        /// </summary>
        public async Task GoBackServer()
        {
            if (сurrentDirectoryOnServer == "")
            {
                ThrowError(this, "Can't go back from the root directory");
                return;
            }

            try
            {
                var index = сurrentDirectoryOnServer.LastIndexOf('\\');
                string toOpen;

                if (index > 0)
                {
                    toOpen = сurrentDirectoryOnServer.Substring(0, сurrentDirectoryOnServer.LastIndexOf('\\'));
                }
                else
                {
                    toOpen = "";
                }

                await TryUpdateCurrentPathsOnServer(toOpen);
            }
            catch (Exception e)
            {
                if (e.Message == "-1")
                {
                    ThrowError(this, "Directory not found exception occured");
                    return;
                }

                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Обновляет папку для загрузки
        /// </summary>
        public void UpdateDownloadFolder()
        {
            if (downloadPath == currentDirectoryOnClientPath)
            {
                return;
            }

            DownloadFolder = currentDirectoryOnClientPath + "\\";
        }

        /// <summary>
        /// Загружает файл с данным именем
        /// </summary>
        public async Task DownloadFile(string fileName)
        {
            try
            {
                var pathToFile = Path.Combine(сurrentDirectoryOnServer, fileName);

                DownloadsInProgressList.Add(fileName);

                await client.Get(pathToFile, downloadPath);

                DownloadsInProgressList.Remove(fileName);
                DownloadsFinishedList.Add(fileName);
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }

        /// <summary>
        /// Загружает все файлы из текущей папки
        /// </summary>
        public async Task DownloadAllFilesInCurrentDirectory()
        {
            try
            {
                foreach (var path in currentPathsOnServer)
                {
                    if (!path.Item2)
                    {
                        await DownloadFile(path.Item1);
                    }
                }
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }
        }
    }
}
