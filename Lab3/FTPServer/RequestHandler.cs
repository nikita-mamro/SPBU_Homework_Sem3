using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Handlers
{
    public static class RequestHandler
    {
        public static byte[] HandleRequest(string request)
        {
            if (int.TryParse(request[0].ToString(), out var id))
            {
                var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, request.Remove(0, 1));

                if (id == 1)
                {
                    return HandleListRequest(path);
                }
                
                if (id == 2)
                {
                    //return HandleGetRequest(path);
                }
            }

            var error = "Wrong format error";
            return Encoding.UTF8.GetBytes(error);
        }

        private static byte[] HandleListRequest(string path)
        {
            if (!Directory.Exists(path))
            {
                var errorResponse = "-1";
                return Encoding.UTF8.GetBytes(errorResponse);
            }

            var response = new StringBuilder();

            var responseSize = 0;

            var files = Directory.GetFiles(path);
            var folders = Directory.GetDirectories(path);

            responseSize = files.Length + folders.Length;

            response.Append($"{responseSize} ");
            
            foreach (var file in files)
            {
                response.Append($"{file} false");
            }

            foreach (var folder in folders)
            {
                response.Append($"{folder} true");
            }

            return Encoding.UTF8.GetBytes(response.ToString());
        }

        private static string HandleGetRequest(string path)
        {
            var response = new StringBuilder();

            return response.ToString();
        }
    }
}
