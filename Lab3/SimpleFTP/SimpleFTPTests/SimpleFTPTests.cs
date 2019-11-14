using Microsoft.VisualStudio.TestTools.UnitTesting;
using FTPClient;
using FTPServer;
using System.IO;

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
        string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "res\\Downloads\\");

        [TestInitialize]
        public void Initialize()
        {
            server = new Server(8888);
            client = new Client("127.0.0.1", 8888);
        }

        [TestMethod]
        public void SimpleFTPTest()
        {
            Assert.Fail();
        }
    }
}