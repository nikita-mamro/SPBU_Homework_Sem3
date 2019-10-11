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
                tasks.Add(myThreadPool.QueueTask(Kek));
            }

            //myThreadPool.Shutdown();

            Thread.Sleep(15000);

            foreach (var task in tasks)
            {
                Console.WriteLine(task.Result);
                Console.WriteLine(task.IsCompleted);
            }
        }

        static int Kek()
        {
            Thread.Sleep(5000);
            Console.WriteLine("Calculating task");
            return 5 * 5;
        }
    }
}
