using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTPClient;
using FTPServer;

namespace SimpleFTP.Tests
{
    [TestClass]
    public class SimpleFTPTests
    {
        Server server;
        Client client;

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