using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace FTPServer
{
    public class FTPServer
    {
        private IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        private int port;
        private TcpListener listener;

        public FTPServer(int port)
        {
            this.port = port;
            listener = new TcpListener(localAddress, port);
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(HandleTcpClient, listener);
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        private void HandleTcpClient(IAsyncResult result)
        {
            var client = listener.EndAcceptTcpClient(result);
            listener.BeginAcceptTcpClient(HandleTcpClient, listener);

        }

        private async Task List()
        {

        }

        private async Task Get()
        {

        }
    }
}
