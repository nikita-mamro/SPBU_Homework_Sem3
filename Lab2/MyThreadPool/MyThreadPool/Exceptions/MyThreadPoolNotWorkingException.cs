using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// Исключение, которое будет бросаться при попытке доступа к
    /// пулу, который уже не работает
    /// </summary>
    [Serializable]
    public class MyThreadPoolNotWorkingException : Exception
    {
        public MyThreadPoolNotWorkingException() : base() { }

        public MyThreadPoolNotWorkingException(string message) : base(message) { }
    }
}
