using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FTPClient;
using Microsoft.Win32;

namespace ViewModel
{
    public class ClientViewModel
    {
        private int port;
        private string server;

        private Client client;
        public bool IsConnected;

        public string RootServerDirectory;
        public string RootClientDirectory;

        public string CurrentDirectory;
        public string PreviousDirectory;

        private string сurrentDirectoryOnServer;

        private ObservableCollection<string> currentPaths;

        public ObservableCollection<string> DisplayedList;

        public EventHandler<string> ErrorHandler;

        public bool IsBackAvailable
            => PreviousDirectory != null;

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
            RootServerDirectory = "..\\..\\..\\Server\\res";
            RootClientDirectory = "..\\..\\..\\Client\\res\\Downloads\\";
            CurrentDirectory = RootServerDirectory;
            сurrentDirectoryOnServer = "";
            PreviousDirectory = null;

            DisplayedList = new ObservableCollection<string>();
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }

            client = new Client(server, port);

            DisplayedList.Clear();

            try
            {
                client.Connect();
                IsConnected = true;
                await InitializeCurrentPaths();
            }
            catch (Exception e)
            {
                ErrorHandler.Invoke(this, e.Message);
            }
        }

        private async Task InitializeCurrentPaths()
        {
            currentPaths = new ObservableCollection<string>();

            currentPaths.CollectionChanged += CurrentPathsChanged;

            await TryUpdateCurrentPaths("");
        }

        private void CurrentPathsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    DisplayedList.Remove(item.ToString());
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    DisplayedList.Add(item.ToString());
                }
            }
        }

        public async Task OpenFolderOrDownloadFile(string folderName)
        {
            var nextDirectory = Path.Combine(CurrentDirectory, folderName);

            if (Directory.Exists(nextDirectory))
            {
                try
                {
                    await TryUpdateCurrentPaths(Path.Combine(сurrentDirectoryOnServer, folderName));
                    CurrentDirectory = nextDirectory;
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

        private async Task TryUpdateCurrentPaths(string listRequest)
        {
            var serverList = await client.List(listRequest);

            while (currentPaths.Count > 0)
            {
                currentPaths.RemoveAt(currentPaths.Count - 1);
            }

            foreach (var path in serverList)
            {
                var name = path.Item1;

                name = name.Substring(name.LastIndexOf('\\') + 1);

                currentPaths.Add(name);
            }
        }

        public async Task GoBack()
        {
            if (сurrentDirectoryOnServer == "")
            {
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

                await TryUpdateCurrentPaths(toOpen);
                сurrentDirectoryOnServer = toOpen;
                CurrentDirectory = Directory.GetParent(CurrentDirectory).ToString();
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

        public async Task DownloadFile(string fileName)
        {
            var pathToFile = Path.Combine(сurrentDirectoryOnServer, fileName);

            //await client.Get(pathToFile, RootClientDirectory);
        }

        public async Task DownloadAllFilesInCurrentDirectory()
        {
        }
    }
}
