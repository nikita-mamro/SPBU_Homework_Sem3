using System;
using System.Threading;
using System.Collections.Generic;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new MyThreadPool(5);

                tasks.Add(pool.QueueTask(() => 2 * 2));
                Console.WriteLine(tasks[i].Result);
                Console.WriteLine(tasks[i].IsCompleted);
            }

            pool.Shutdown();
        }
    }
}
