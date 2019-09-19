using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy
{
    public class Lazy<T> : ILazy<T>
    {
        private bool isCounted = false;
        private Func<T> supplier;
        private T value;

        public Lazy(Func<T> supplier)
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
                value = supplier();
                isCounted = true;
            }

            return value;
        }
    }
}
