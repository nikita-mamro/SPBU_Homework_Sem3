using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        public T Get()
        {
            return default(T);
        }
    }
}
