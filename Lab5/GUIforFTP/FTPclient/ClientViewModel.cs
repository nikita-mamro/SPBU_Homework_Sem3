using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTPClient;

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

        public string DownloadDirectory;

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

        public ClientViewModel(string server, int port)
        {
            this.port = port;
            this.server = server;
            RootServerDirectory = "..\\..\\..\\Server\\res";
            RootClientDirectory = "..\\..\\..\\Client\\res\\Downloads";
            CurrentDirectory = RootServerDirectory;
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

            foreach (var item in e.NewItems)
            {
                DisplayedList.Add(item.ToString());
            }
        }

        public async Task OpenFolderOrDownloadFile(string folderName)
        {
            var nextDirectory = Path.Combine(CurrentDirectory, folderName);

            if (Directory.Exists(nextDirectory))
            {
                try
                {
                    await TryUpdateCurrentPaths(folderName);
                    CurrentDirectory = nextDirectory;
                }
                catch (Exception e)
                {
                    ErrorHandler.Invoke(this, e.Message);
                }
            }
            else
            {
                ErrorHandler.Invoke(this, "TODO:downloadfile");
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
                var isFolder = path.Item2;
                var name = path.Item1.Replace("\\", string.Empty).Remove(0, 1);

                currentPaths.Add(name);
            }
        }

        public async Task GoBack()
        {
            await TryUpdateCurrentPaths("");
        }

        public async Task DownloadFile(string fileName)
        {

        }
    }
}
