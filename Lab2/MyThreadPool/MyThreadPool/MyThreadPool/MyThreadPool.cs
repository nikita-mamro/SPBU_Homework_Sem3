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
        /// Возвращает токен, отвечающий за отмену работы пула
        /// </summary>
        private CancellationTokenSource cts;

        /// <summary>
        /// Указывает на то, работает ли пул
        /// </summary>
        public bool IsWorking
            => !cts.IsCancellationRequested;

        /// <summary>
        /// Объект, с помощью которого подаём сигналы потокам
        /// при добавлении в очередь очередной задачи
        /// </summary>
        private AutoResetEvent taskQueryWaiter;

        /// <summary>
        /// Объект, с помощью которого подаём сигнал
        /// о готовности к заверешинию работы пула
        /// </summary>
        private AutoResetEvent readyToCloseWaiter;

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
                            if (taskQueue.Count == 0)
                            {
                                break;
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
        private IMyTask<TResult> QueueMyTask<TResult>(MyTask<TResult> task)
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

        /// <summary>
        /// Класс, реализующий интерфейс IMyTask
        /// </summary>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            /// <summary>
            /// Объект, предоставляющий вычисление
            /// </summary>
            private Func<TResult> supplier;

            /// <summary>
            /// Результат вычисления
            /// </summary>
            private TResult result;

            /// <summary>
            /// Флаг, указывающий на то, посчитан ли результат
            /// </summary>
            public bool IsCompleted { get; private set; } = false;

            /// <summary>
            /// Объект для блокировки задачи
            /// </summary>
            private object taskLock = new object();

            /// <summary>
            /// Объект, с помощью которого подаём сигналы потокам
            /// о готовности результата задачи
            /// </summary>
            private AutoResetEvent resultWaiter;

            /// <summary>
            /// Пул потоков, в котором выполняется MyTask
            /// </summary>
            private MyThreadPool pool;

            /// <summary>
            /// Очередь для задач из ContinueWith
            /// </summary>
            private ConcurrentQueue<Action> taskQueue;

            /// <summary>
            /// Обработчик брошенных исключений
            /// </summary>
            private Exception exceptionHandler;

            public TResult Result
            {
                get
                {
                    // Здесь ждём, пока считается результат
                    if (!IsCompleted)
                    {
                        resultWaiter.WaitOne();
                    }

                    if (exceptionHandler == null)
                    {
                        return result;
                    }

                    throw exceptionHandler;
                }
                private set
                    => result = value;
            }

            /// <summary>
            /// При создании MyTask требуем предоставить объект для вычислений,
            /// а также указать, в каком пуле таск будет исполняться
            /// </summary>
            public MyTask(Func<TResult> supplier, MyThreadPool pool)
            {
                this.supplier = supplier;
                this.pool = pool;
                resultWaiter = new AutoResetEvent(false);
                taskQueue = new ConcurrentQueue<Action>();
            }

            /// <summary>
            /// Само вычисление
            /// </summary>
            public void Calculate()
            {
                // Попытка выполнить задачу и в случае чего перехватить исключение
                try
                {
                    result = supplier();
                    supplier = null;
                }
                catch (Exception supplierException)
                {
                    exceptionHandler = supplierException;
                }

                lock (taskLock)
                {
                    if (exceptionHandler == null)
                    {
                        IsCompleted = true;
                    }

                    // Уведомляем о том, что задача выполнена
                    resultWaiter.Set();

                    // В случае, если текущая задача 
                    // должна продолжиться другими, исполняем их
                    ProcessTasksFromQueue();
                }
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
            {
                var nextTask = new MyTask<TNewResult>(() => supplier(result), pool);

                lock (taskLock)
                {
                    if (!IsCompleted)
                    {
                        taskQueue.Enqueue(nextTask.Calculate);
                        return nextTask;
                    }
                }

                return pool.QueueMyTask(nextTask);
            }

            /// <summary>
            /// Выполнение задач, поставленных в очередь методом ContinueWith()
            /// </summary>
            private void ProcessTasksFromQueue()
            {
                // Если в очереди не стоят никакие задачи, сразу выходим
                if (taskQueue.Count == 0)
                {
                    return;
                }

                if (exceptionHandler == null)
                {
                    // Если работа пула завершена, задача не выполнится
                    if (!pool.IsWorking)
                    {
                        return;
                    }

                    foreach (var task in taskQueue)
                    {
                        task();
                    }

                    return;
                }

                //Если в какой-то из задач в очереди бросается исключение, перекидываем его дальше
                throw exceptionHandler;
            }
        }
    }
}
