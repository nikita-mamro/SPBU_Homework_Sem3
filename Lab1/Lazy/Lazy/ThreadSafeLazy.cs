using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private volatile bool isCounted = false;
        private Func<T> supplier;
        private T value;
        private object _lock = new object();

        public ThreadSafeLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException();
            }

            this.supplier = supplier;
        }

        public T Get()
        {
            if (!isCounted)
            {
                lock (_lock)
                {
                    if (isCounted)
                    {
                        return value;
                    }

                    value = supplier();
                    isCounted = true;
                }
            }

            return value;
        }
    }
}
