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

        private void ErrorCatcher(object sender, string errorMessage)
        {
            Assert.IsTrue(true);
        }

        [TestInitialize]
        public void Initialize()
        {
            address = "127.0.0.1";
            port = 9999;
            server = new Server(port);

            model = new ClientViewModel(folderPath);
            model.Port = "9999";
            model.Address = "127.0.0.1";
            model.ErrorHandler += ErrorCatcher;
        }

        [TestMethod]
        public void ConnectTest()
        {
            Task.Run(async () =>
            {
                await model.Connect();

                Assert.Fail();
            });
        }

        [TestMethod]
        public void OpenClientFolderTest()
        {
            server.Start();
            model.Connect();

            model.OpenClientFolder("Folllder");
            model.OpenClientFolder("Folder");

            Assert.IsTrue(model.DisplayedListOnClient.Contains("FolderInFolder"));

            model.GoBackClient();

            Assert.IsTrue(model.DisplayedListOnClient.Contains("Folder"));
        }
        
        [TestMethod]
        public void OpenServerFolderTest()
        {
            server.Start();
            model.Connect();

            model.OpenServerFolderOrDownloadFile("TestData").Wait();

            var res = model.DisplayedListOnServer;

            Assert.IsTrue(model.DisplayedListOnServer.Contains(""));
        }
        
        //[TestMethod]
        //public void GoBackClientTest()
        //{
        //    Assert.Fail();
        //}
        //
        //[TestMethod]
        //public void GoBackServerTest()
        //{
        //    Assert.Fail();
        //}
        //
        //[TestMethod]
        //public void UpdateDownloadFolderTest()
        //{
        //    Assert.Fail();
        //}
        //
        //[TestMethod]
        //public void DownloadFileTest()
        //{
        //    Assert.Fail();
        //}
        //
        //[TestMethod]
        //public void DownloadAllFilesInCurrentDirectoryTest()
        //{
        //    Assert.Fail();
        //}
    }
}