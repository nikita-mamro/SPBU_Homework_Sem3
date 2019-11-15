using Microsoft.VisualStudio.TestTools.UnitTesting;
using FTPClient;
using FTPServer;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleFTP.Tests
{
    /// <summary>
    /// Тесты корректной работы сервера и клиента
    /// </summary>
    [TestClass]
    public class SimpleFTPTests
    {
        Server server;
        Client client;
        string rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        string folderPath = "res\\Downloads\\";
        string pathToDownloaded;
        List<(string, bool)> expectedListTest;
        List<(string, bool)> expectedListTestFolder;

        [TestInitialize]
        public void Initialize()
        {
            pathToDownloaded = Path.Combine(rootPath, folderPath);
            server = new Server(8888);
            client = new Client("127.0.0.1", 8888);

            expectedListTest.Add((".\\Test\\testTxt.txt", false));
            expectedListTest.Add((".\\Test\\testWord.docx", false));
            expectedListTest.Add((".\\Test\\testFolder", true));

            expectedListTestFolder.Add((".\\Test\\testFolder\\someText.txt", false));
        }

        [TestMethod]
        public void SimpleFTPTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ListTest()
        {
            Task.Run(async () => await server.Start());

            var listTest = client.List("Test").Result;

            for (var i = 0; i < expectedListTestFolder.Count; ++i)
            {
                Assert.AreEqual(expectedListTestFolder[i], listTest[i]);
            }
        }

        [TestMethod]
        public void ListFolderInFolderTest()
        {
            Task.Run(async () => await server.Start());

            var listTestFolder = client.List("Test\\testFolder").Result;

            for (var i = 0; i < expectedListTestFolder.Count; ++i)
            {
                Assert.AreEqual(expectedListTestFolder[i], listTestFolder[i]);
            }
        }
    }
}