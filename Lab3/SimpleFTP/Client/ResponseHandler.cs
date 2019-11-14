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
                throw new Exception(response);
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
