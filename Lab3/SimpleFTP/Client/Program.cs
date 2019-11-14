using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client("127.0.0.1", 8888);
            await client.Start();
        }
    }
}
