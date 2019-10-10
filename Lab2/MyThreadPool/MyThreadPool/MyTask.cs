using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public TResult Result
        {
            get
            {
                resultWaiter.WaitOne();
                return result;
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
