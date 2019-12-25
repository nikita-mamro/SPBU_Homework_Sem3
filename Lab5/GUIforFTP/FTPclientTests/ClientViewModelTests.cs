using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FTPServer;

namespace ViewModel.Tests
{
    /// <summary>
    /// Тесты бэкенда
    /// </summary>
    [TestClass]
    public class ClientViewModelTests
    {
        private Server server;
        private string address;
        private int port;
        private string folderPath = "..\\..\\res\\Downloads\\";

        private ClientViewModel model;

        private bool passed = false;

        private void ErrorCatcher(object sender, string errorMessage)
        {
            passed = true;
        }
        
        [TestInitialize]
        public void Initialize()
        {
            address = "127.0.0.1";
            port = 9999;
            server = new Server(port);

            model = new ClientViewModel(folderPath);
            model.Port = "9999";
            model.Address = address;
            model.ThrowError += ErrorCatcher;
        }

        [TestMethod]
        public void ConnectTest()
        {
            model.IsConnected = false;

            Task.Run(async () =>
            {
                await model.Connect();

                Assert.Fail();
            });
        }

        [TestMethod]
        public void OpenClientFolderTest()
        {
            model.IsConnected = false;

            server.Start();
            model.Connect();

            model.OpenClientFolder("Folllder");
            model.OpenClientFolder("Folder");

            Assert.IsTrue(model.DisplayedListOnClient.Contains("FolderInFolder"));
        }

        [TestMethod]
        public void GoBackFromRootClientExceptionTest()
        {
            model.IsConnected = false;

            server.Start();
            model.Connect();

            model.GoBackClient();

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void GoBackFromRootServerExceptionTest()
        {
            model.IsConnected = false;

            server.Start();
            model.Connect();

            model.GoBackServer();

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void GoBackClientTest()
        {
            model.IsConnected = false;

            server.Start();
            model.Connect();

            model.OpenClientFolder("Folder");

            model.GoBackClient();

            Assert.IsTrue(model.DisplayedListOnClient.Contains("Folder"));
        }
    }
}