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

        private int closedThreads = 0;
        private object lockObject = new object();

        /// <summary>
        /// Очередь задач на выполнение
        /// </summary>
        private ConcurrentQueue<Action> taskQueue;

        /// <summary>
        /// Возвращает токен, отвечающий за отмену работы пула
        /// </summary>
        private CancellationTokenSource cts;

        public bool IsWorking
            => !cts.IsCancellationRequested;

        /// <summary>
        /// Объект, с помощью которого подаём сигналы потокам
        /// при добавлении в очередь очередной задачи
        /// </summary>
        private AutoResetEvent taskQueryWaiter;

        private AutoResetEvent readyToCloseWaiter;

        private object queueLocker = new object();

        /// <summary>
        /// Конструктор, создающий пул с фиксированным числом потков
        /// </summary>
        public MyThreadPool(int numberOfThreads)
        {
            threads = new List<Thread>();
            taskQueue = new ConcurrentQueue<Action>();
            taskQueryWaiter = new AutoResetEvent(false);
            readyToCloseWaiter = new AutoResetEvent(false);
            cts = new CancellationTokenSource();

            for (var i = 0; i < numberOfThreads; ++i)
            {
                threads.Add(new Thread(() =>
                {
                    while (true)
                    {
                        if (cts.IsCancellationRequested)
                        {
                            lock (queueLocker)
                            {
                                if (taskQueue.Count == 0)
                                {
                                    break;
                                }
                            }
                        }

                        // Выполняем то, что появляется в очереди
                        if (taskQueue.TryDequeue(out var calculateTask))
                        {
                            calculateTask();
                        }
                        else
                        {
                            // Переводим исполнителя в состояние ожидания,
                            // пока в очередь не добавится MyTask 
                            taskQueryWaiter.WaitOne();
                        }
                    }

                    // Подаём сигнал о том, что можно заканчивать работу
                    readyToCloseWaiter.Set();
                }));

                threads[i].Start();
            }
        }

        /// <summary>
        /// Завершает работу потоков в пуле
        /// Если есть незавершённые задачи, даём им завершиться
        /// </summary>
        public void Shutdown()
        {
            if (!IsWorking)
            {
                throw new MyThreadPoolNotWorkingException("Pool is already not working.");
            }

            cts.Cancel();

            taskQueryWaiter.Set();
            readyToCloseWaiter.WaitOne();

            foreach (var thread in threads)
            {
                thread.Abort();
            }

            taskQueue = null;
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
                throw new MyThreadPoolNotWorkingException("Пул потоков не работает, невозможно поставить новую задачу в очередь на исполнение.");
            }
            
            // Добавляем задачу в очередь на исполнение
            taskQueue.Enqueue(task.Calculate);
            // Даём исполнителю задачи сигнал, если он в ожидании
            taskQueryWaiter.Set();

            return task;
        }
    }
}
