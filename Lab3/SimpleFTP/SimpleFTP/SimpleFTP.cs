using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTPServer;
using FTPClient;

namespace SimpleFTP
{
    public class SimpleFTP
    {
        public Server server;
        public Client client;

        public SimpleFTP()
        {
            server = new Server(8888);
            client = new Client("127.0.0.1", 8888);
        }
    }
}
