using System;
using System.Collections.Generic;
using System.IO;

namespace FTPClient
{
    /// <summary>
    /// Класс, отвечающий за обработку ответов от сервера
    /// </summary>
    public static class ResponseHandler
    {
        /// <summary>
        /// Преобразование строкового ответа на запрос List в список пар (string, bool)
        /// </summary>
        public static List<(string, bool)> HandleListResponse(string response)
        {
            var splitResponse = response.Split(' ');

            int resultLength;

            if (int.TryParse(splitResponse[0], out var res))
            {
                resultLength = res;
            }
            else
            {
                throw new ArgumentException(response);
            }

            if (resultLength == -1)
            {
                throw new DirectoryNotFoundException(response);
            }

            var result = new List<(string, bool)>();

            for (var i = 1; i < resultLength * 2; i += 2)
            {
                result.Add((splitResponse[i], bool.Parse(splitResponse[i + 1])));
            }

            return result;
        }
    }
}
