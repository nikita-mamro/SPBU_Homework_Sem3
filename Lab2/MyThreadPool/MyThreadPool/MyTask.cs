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
        /// Очередь для задач из ContinueWith (планируется по крайней мере так использовать)
        /// </summary>
        private ConcurrentQueue<Action> taskQueue;

        public TResult Result
        {
            get
            {
                // Здесь ждём, пока считается результат
                resultWaiter.WaitOne();
                return result;
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
            result = supplier();

            lock (taskLock)
            {    
                IsCompleted = true;
                // Уведомляем о том, что результат посчитан
                resultWaiter.Set();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
        {
            var nextTask = new MyTask<TNewResult>(() => supplier(result), pool);

            lock (taskLock)
            {
                if (!IsCompleted)
                {

                }
            }

            return pool.QueueMyTask(nextTask);
        }
    }
}
