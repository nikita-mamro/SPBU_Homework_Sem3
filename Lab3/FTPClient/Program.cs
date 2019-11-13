using System;
using System.Threading.Tasks;

namespace FTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new FTPClient("127.0.0.1", 8888);
            await client.Start();
        }
    }
}
