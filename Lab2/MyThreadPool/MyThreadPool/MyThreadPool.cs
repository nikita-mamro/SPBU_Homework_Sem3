using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс, реализующий пул потоков
    /// </summary>
    public class MyThreadPool
    {
        /// <summary>
        /// Список потоков в пуле
        /// </summary>
        private List<Thread> threads;

        /// <summary>
        /// Очередь задач на выполнение
        /// </summary>
        private ConcurrentQueue<Action> taskQueue;

        /// <summary>
        /// 
        /// </summary>
        private CancellationTokenSource cts;

        /// <summary>
        /// Объект, с помощью которого подаём сигналы потокам
        /// при добавлении в очередь очередной задачи
        /// </summary>
        private AutoResetEvent taskQueryWaiter;

        /// <summary>
        /// Конструктор, создающий пул с фиксированным числом потков
        /// </summary>
        public MyThreadPool(int numberOfThreads)
        {
            threads = new List<Thread>();
            taskQueue = new ConcurrentQueue<Action>();
            taskQueryWaiter = new AutoResetEvent(false);
            cts = new CancellationTokenSource();

            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        // Блокируем исполнителя, пока поставщик задач не добавит в очередь MyTask 
                        taskQueryWaiter.WaitOne();

                        // Выполняем то, что появилось в очереди
                        if (taskQueue.TryDequeue(out var calculateTask))
                        {
                            calculateTask();
                        }
                    }
                }));

                threads[i].Start();
            }
        }

        /// <summary>
        /// Завершает работу потоков в пуле
        /// (пока как-то не очень выглядит)
        /// </summary>
        public void Shutdown()
        {
            cts.Cancel();

            taskQueue = null;

            foreach (var thread in threads)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Ставит в очередь задачу, выполняющую переданное вычисление
        /// </summary>
        public IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            return QueueMyTask(new MyTask<TResult>(supplier, this));
        }

        /// <summary>
        /// Ставит в очередь переданную задачу MyTask
        /// </summary>
        public IMyTask<TResult> QueueMyTask<TResult>(MyTask<TResult> task)
        {
            if (cts.IsCancellationRequested)
            {
                throw new Exception();
            }
            
            // Добавляем задачу в очередь на исполнение
            taskQueue.Enqueue(task.Calculate);
            // Даём исполнителю задачи сигнал, если он в ожидании
            taskQueryWaiter.Set();

            return task;
        }
    }
}
