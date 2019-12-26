using System.Threading.Tasks;

namespace FTPServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server(8888);
            await server.Start();
        }
    }
}
