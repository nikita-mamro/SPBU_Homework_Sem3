using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private readonly int threadsCapacity;

        private List<Thread> threads;
        private ConcurrentQueue<Action> taskQueue;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private AutoResetEvent taskQueryWaiter = new AutoResetEvent(true);

        private object locker = new object();

        public MyThreadPool(int numberOfThreads)
        {
            threads = new List<Thread>(numberOfThreads);
            taskQueue = new ConcurrentQueue<Action>();

            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        if (taskQueue.TryDequeue(out var task))
                        {
                            task();
                        }
                        else
                        {
                            taskQueryWaiter.WaitOne();
                        }
                    }
                }));

                threads[i].Start();
            }
        }

        public void Shutdown()
        {
            cts.Cancel();
        }

        public MyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            return QueueMyTask(new MyTask<TResult>(supplier));
        }

        private MyTask<TResult> QueueMyTask<TResult>(MyTask<TResult> task)
        {
            if (cts.IsCancellationRequested)
            {
                throw new Exception();
            }

            taskQueue.Enqueue(task.Calculate);
            taskQueryWaiter.Set();
            return task;
        }
    }
}
