using System;
using System.Collections.Generic;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var myThreadPool = new MyThreadPool(10);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 20; i++)
            {

                tasks.Add(myThreadPool.QueueTask(() =>
                {
                    Console.WriteLine($"Proceeding task {i}");
                    return 2 * 2;
                }));

                Console.WriteLine(tasks[i].Result);
                Console.WriteLine(tasks[i].IsCompleted);

            }

            myThreadPool.Shutdown();
        }
    }
}
