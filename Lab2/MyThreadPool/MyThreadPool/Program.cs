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

        static void LocalTest1()
        {
            var myThreadPool = new MyThreadPool(10);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 15; ++i)
            {
                tasks.Add(myThreadPool.QueueTask(Kek));
            }

            Thread.Sleep(2500);

            myThreadPool.Shutdown();

            foreach (var task in tasks)
            {
                Console.WriteLine(task.Result);
                Console.WriteLine(task.IsCompleted);
            }
        }

        static int Kek()
        {
            var answer = 5 * 5;

            Thread.Sleep(1000);
            Console.WriteLine("Finished calculating");

            return answer;
        }
    }
}
