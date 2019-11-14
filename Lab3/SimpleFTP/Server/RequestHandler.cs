using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FTPServer
{
    public static class RequestHandler
    {
        public async static Task HandleRequest(string request, StreamWriter writer)
        {
            if (int.TryParse(request[0].ToString(), out var id))
            {
                var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, request.Remove(0, 1));

                if (id == 1)
                {
                    await HandleListRequest(path, writer);
                }

                if (id == 2)
                {
                    await HandleGetRequest(path, writer);
                }
            }

            var error = "Wrong format error";
            await writer.WriteLineAsync(error);
        }

        private static async Task HandleListRequest(string path, StreamWriter writer)
        {
            if (!Directory.Exists(path))
            {
                var errorResponse = "-1";
                await writer.WriteLineAsync(errorResponse);
                return;
            }

            var response = new StringBuilder();

            var responseSize = 0;

            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);

            responseSize = files.Length + folders.Length;

            response.Append($"{responseSize} ");

            foreach (var file in files)
            {
                response.Append($"{file} false ");
            }

            foreach (var folder in folders)
            {
                response.Append($"{folder} true ");
            }

            await writer.WriteLineAsync(response.ToString());
        }

        private static async Task HandleGetRequest(string path, StreamWriter writer)
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1");
            }
        }
    }
}
