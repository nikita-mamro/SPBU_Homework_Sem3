using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FTPClient;
using Microsoft.Win32;

namespace ViewModel
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private int port;
        private string server;

        private Client client;
        public bool IsConnected;

        public string RootServerDirectoryPath;
        public string RootClientDirectoryPath;

        /// <summary>
        /// Полный путь до текущей папки на сервере
        /// </summary>
        private string currentDirectoryOnServerPath;

        private string currentDirectoryOnClientPath;

        /// <summary>
        /// Путь до текушей папки на сервере, считая от корневой
        /// </summary>
        private string сurrentDirectoryOnServer;

        private string currentDirectoryOnClient;

        private string downloadPath;

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

        private ObservableCollection<string> currentPathsOnServer;
        private ObservableCollection<string> currentPathsOnClient;

        public ObservableCollection<string> DisplayedListOnServer;
        public ObservableCollection<string> DisplayedListOnClient;

        public EventHandler<string> ErrorHandler;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string Address
        {
            get
                => server;
            set
                => server = value;
        }

        public ClientViewModel()
        {
            this.server = "127.0.0.1";
            this.port = 8888;
            RootServerDirectoryPath = "..\\..\\..\\Server\\res";
            RootClientDirectoryPath = "..\\..\\..\\Client\\res\\Downloads\\";
            currentDirectoryOnServerPath = RootServerDirectoryPath;
            сurrentDirectoryOnServer = "";

            currentDirectoryOnClientPath = RootClientDirectoryPath;
            currentDirectoryOnClient = "";
            downloadPath = RootClientDirectoryPath;

            DisplayedListOnServer = new ObservableCollection<string>();
            DisplayedListOnClient = new ObservableCollection<string>();

            InitializeCurrentPathsOnClient();
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
                IsConnected = true;
                await InitializeCurrentPathsOnServer();
            }
            catch (Exception e)
            {
                ErrorHandler.Invoke(this, e.Message);
            }
        }

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
                ErrorHandler.Invoke(this, e.Message);
            }
        }

        private async Task InitializeCurrentPathsOnServer()
        {
            currentPathsOnServer = new ObservableCollection<string>();

            currentPathsOnServer.CollectionChanged += CurrentPathsOnServerChanged;
            
            await TryUpdateCurrentPathsOnServer("");
        }

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

        private void CurrentPathsOnServerChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    DisplayedListOnServer.Remove(item.ToString());
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    DisplayedListOnServer.Add(item.ToString());
                }
            }
        }

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
                ErrorHandler.Invoke(this, $"Folder {folderName} does not exist on client");
            }
        }

        public async Task OpenServerFolderOrDownloadFile(string folderName)
        {
            var nextDirectory = Path.Combine(currentDirectoryOnServerPath, folderName);

            if (Directory.Exists(nextDirectory))
            {
                try
                {
                    await TryUpdateCurrentPathsOnServer(Path.Combine(сurrentDirectoryOnServer, folderName));
                    currentDirectoryOnServerPath = nextDirectory;
                    сurrentDirectoryOnServer = Path.Combine(сurrentDirectoryOnServer, folderName);
                }
                catch (Exception e)
                {
                    ErrorHandler.Invoke(this, e.Message);
                }
            }
            else
            {
                try
                {
                    await DownloadFile(folderName);
                }
                catch (Exception e)
                {
                    ErrorHandler.Invoke(this, e.Message);
                }
            }
        }

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
                ErrorHandler.Invoke(this, e.Message);
            }
        }

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

                    currentPathsOnServer.Add(name);
                }
            }
            catch (Exception e)
            {
                ErrorHandler.Invoke(this, e.Message);
            }
        }

        public void GoBackClient()
        {
            if (currentDirectoryOnClient == "")
            {
                ErrorHandler.Invoke(this, "Can't go back from the root directory");
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
                ErrorHandler.Invoke(this, e.Message);
            }
        }

        public async Task GoBackServer()
        {
            if (сurrentDirectoryOnServer == "")
            {
                ErrorHandler.Invoke(this, "Can't go back from the root directory");
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
                сurrentDirectoryOnServer = toOpen;
                currentDirectoryOnServerPath = Directory.GetParent(currentDirectoryOnServerPath).ToString();
            }
            catch (Exception e)
            {
                if (e.Message == "-1")
                {
                    ErrorHandler.Invoke(this, "Directory not found exception occured");
                    return;
                }

                ErrorHandler.Invoke(this, e.Message);
            }
        }

        public void UpdateDownloadFolder()
        {
            if (downloadPath == currentDirectoryOnClientPath)
            {
                return;
            }

            DownloadFolder = currentDirectoryOnClientPath + "\\";
        }

        public async Task DownloadFile(string fileName)
        {
            var pathToFile = Path.Combine(сurrentDirectoryOnServer, fileName);

            await client.Get(pathToFile, downloadPath);
        }

        public async Task DownloadAllFilesInCurrentDirectory()
        {
        }
    }
}
