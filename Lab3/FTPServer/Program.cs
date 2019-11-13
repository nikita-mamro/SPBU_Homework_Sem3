using System;
using System.Threading.Tasks;

namespace FTPServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new FTPServer(8888);

            await server.Start();
        }
    }
}
