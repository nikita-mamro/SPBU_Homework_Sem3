using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient
{
    public static class ResponseHandler
    {
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
                throw new Exception(response);
            }

            if (resultLength == -1)
            {
                throw new Exception("-1");
            }

            var result = new List<(string, bool)>();

            for (var i = 0; i < resultLength; i += 2)
            {
                result.Add((splitResponse[i], bool.Parse(splitResponse[i + 1])));
            }

            return result;
        }
    }
}
