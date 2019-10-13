using System;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new MyThreadPool(10);

            var task = pool.QueueTask(() =>
            {
                Thread.Sleep(1000);
                return 4;
            });

            pool.Shutdown();

            Console.WriteLine(task.IsCompleted);
            //Console.WriteLine(task.Result);
        }
    }
}
