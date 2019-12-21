using Microsoft.VisualStudio.TestTools.UnitTesting;
using FTPClient;
using FTPServer;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace SimpleFTP.Tests
{
    /// <summary>
    /// Тесты корректной работы сервера и клиента
    /// </summary>
    [TestClass]
    public class SimpleFTPTests
    {
        private Server server;
        private Client client;
        private string address;
        private int port;
        private string rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        private string folderPath = "res\\Downloads\\";
        private string pathToDownloaded;
        private List<(string, bool)> expectedListTest;
        private List<(string, bool)> expectedListTestFolder;

        [TestInitialize]
        public void Initialize()
        {
            port = 9999;
            address = "127.0.0.1";

            pathToDownloaded = Path.Combine(rootPath, folderPath);
            server = new Server(port);
            client = new Client(address, port);

            expectedListTest = new List<(string, bool)>();
            expectedListTestFolder = new List<(string, bool)>();

            expectedListTest.Add((".\\Test\\testTxt.txt", false));
            expectedListTest.Add((".\\Test\\testWord.docx", false));
            expectedListTest.Add((".\\Test\\testFolder", true));

            expectedListTestFolder.Add((".\\Test\\testFolder\\someText.txt", false));
        }

        [TestMethod]
        public void ListTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            var listTest = client.List("Test").Result;

            server.Stop();
            client.Stop();

            for (var i = 0; i < expectedListTest.Count; ++i)
            {
                Assert.AreEqual(expectedListTest[i], listTest[i]);
            }
        }

        [TestMethod]
        public void ListFolderInFolderTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            var listTestFolder = client.List("Test\\testFolder").Result;

            server.Stop();
            client.Stop();

            for (var i = 0; i < expectedListTestFolder.Count; ++i)
            {
                Assert.AreEqual(expectedListTestFolder[i], listTestFolder[i]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ListDirectoryExceptionTest()
        {
            Task.Run(async () => await server.Start());

            client.Connect();

            var res = client.List("ololo").Result;

            server.Stop();
            client.Stop();
        }

        [TestMethod]
        public void GetTest()
        {
            var pathToFile = Path.Combine(pathToDownloaded, "testTxt.txt");

            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }

            Task.Run(async () =>
            {
                await server.Start();

                client.Connect();

                await client.Get("Test\\testTxt.txt", pathToDownloaded);

                server.Stop();

                Assert.IsTrue(File.Exists(pathToFile));

                File.Delete(pathToFile);

                client.Stop();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetFileNotFoundTest()
        {
            Task.Run(async () => await server.Start());
            
            client.Connect();

            client.Get("olololo", pathToDownloaded).Wait();

            server.Stop();
            client.Stop();
        }

        [TestMethod]
        public void MultipleTaskTest()
        {
            var pathToFile = Path.Combine(pathToDownloaded, "testWord.docx");

            if (File.Exists(pathToFile))
            {
                File.Delete(pathToFile);
            }

            Task.Run(async () =>
            {
                await server.Start();

                client.Connect();

                await client.Get("Test\\testWord.docx", pathToDownloaded);

                Assert.IsTrue(File.Exists(pathToFile));

                var listTestFolder = client.List("Test\\testFolder").Result;

                server.Stop();
                client.Stop();

                for (var i = 0; i < expectedListTestFolder.Count; ++i)
                {
                    Assert.AreEqual(expectedListTestFolder[i], listTestFolder[i]);
                }
            });
        }
    }
}