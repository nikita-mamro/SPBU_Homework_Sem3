using System;
using System.Threading;
using System.Collections.Concurrent;

namespace MyThreadPool
{
    /// <summary>
    /// Класс, реализующий интерфейс IMyTask
    /// </summary>
    public class MyTask<TResult> : IMyTask<TResult>
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

        private Exception exceptionHandler;

        public TResult Result
        {
            get
            {
                // Здесь ждём, пока считается результат
                resultWaiter.WaitOne();
                
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
        /// а также указать, в каком пуле будет исполняться
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
                try
                {
                    ProcessTasksFromQueue();
                }
                catch (Exception e)
                {
                    throw e;
                }
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
