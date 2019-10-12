using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private int currentThreads;

        private List<Thread> threads;
        private ConcurrentQueue<Action> taskQueue;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private AutoResetEvent taskQueryWaiter = new AutoResetEvent(true);
        private AutoResetEvent closeWaiter = new AutoResetEvent(true);

        private Semaphore sem;

        private object closeLocker = new object();
        private object locker = new object();

        public MyThreadPool(int numberOfThreads)
        {
            sem = new Semaphore(numberOfThreads, numberOfThreads);

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

                    //lock (closeLocker)
                    //{
                    //    --currentThreads;
                    //}
                    //
                    //if (currentThreads == 0)
                    //{
                    //    closeWaiter.Set();
                    //}

                }));

                threads[i].Start();
            }
        }

        public void Shutdown()
        {
            cts.Cancel();
            taskQueue = null;
            
            foreach (var thread in threads)
            {
                thread.Abort();
            }
        }

        public IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            return QueueMyTask(new MyTask<TResult>(supplier));
        }

        private IMyTask<TResult> QueueMyTask<TResult>(MyTask<TResult> task)
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
