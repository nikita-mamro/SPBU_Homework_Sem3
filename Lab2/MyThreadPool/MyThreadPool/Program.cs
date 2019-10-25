using System;
using System.Threading;
using System.Collections.Generic;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var pool = new MyThreadPool(2);
            var mre = new ManualResetEvent(false);
            var tasks = new List<IMyTask<int>>();

            var thread1 = new Thread(() =>
            {
                mre.WaitOne();
                Thread.Sleep(1000);
                tasks.Add(pool.QueueTask(() => 1));
            });

            var thread2 = new Thread(() =>
            {
                mre.WaitOne();
                Thread.Sleep(1000);
                tasks.Add(pool.QueueTask(() => 2));
            });

            thread1.Start();
            thread2.Start();

            mre.Set();

            thread1.Join();
            thread2.Join();

            pool.Shutdown();

            foreach (var task in tasks)
            {
                Console.WriteLine(task.Result);
            }
        }
    }
}
