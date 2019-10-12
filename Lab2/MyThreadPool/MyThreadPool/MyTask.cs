using System;
using System.Threading;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> supplier;
        private object taskLock = new object();

        public bool IsCompleted { get; private set; } = false;
        private TResult result;

        private AutoResetEvent resultWaiter = new AutoResetEvent(true);

        private MyThreadPool pool;

        public TResult Result
        {
            get
            {
                lock (taskLock)
                {
                    resultWaiter.WaitOne();
                    return result;
                }
            }
            private set
                => Result = value;
        }

        public MyTask(Func<TResult> supplier)
        {
            this.supplier = supplier;
        }

        public void Calculate()
        {
            result = supplier();

            lock (taskLock)
            {
                IsCompleted = true;
                resultWaiter.Set();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier)
        {
            throw new NotImplementedException();
        }
    }
}
