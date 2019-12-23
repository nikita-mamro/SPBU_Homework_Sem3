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
    [TestClass]
    public class ClientViewModelTests
    {
        private Server server;
        private string address;
        private int port;
        private string rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        //private string folderPath = "res\\Downloads\\";
        //private string pathToDownloaded;

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

            model = new ClientViewModel();
            model.ErrorHandler += ErrorCatcher;
        }

        [TestMethod]
        public void ConnectTest()
        {
            //TODO: initial paths test
            Task.Run(async () =>
            {
                await server.Start();
                await model.Connect();
            });

            Assert.Fail();
        }

        //[TestMethod]
        //public void OpenClientFolderTest()
        //{
        //    Assert.Fail();
        //}
        //
        //[TestMethod]
        //public void OpenServerFolderOrDownloadFileTest()
        //{
        //    Assert.Fail();
        //}
        //
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