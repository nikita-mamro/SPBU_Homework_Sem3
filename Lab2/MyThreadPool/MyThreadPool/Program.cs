using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var myThreadPool = new MyThreadPool(10);

            var tasks = new List<MyTask<int>>();

            for (var i = 0; i < 15; ++i)
            {
                tasks.Add(myThreadPool.QueueTask(() => 5 * 5));
            }

            myThreadPool.Shutdown();

            foreach (var task in tasks)
            {
                Console.WriteLine(task.Result);
                Console.WriteLine(task.IsCompleted);
            }
        }
    }
}
