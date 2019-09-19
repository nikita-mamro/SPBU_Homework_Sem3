using System;
using System.Collections.Generic;
using System.Text;

namespace Lazy
{
    public static class LazyFactory<T>
    {
        public static Lazy<T> CreateLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException();
            }

            return new Lazy<T>(supplier);
        }

        //public static ThreadSafeLazy<T> CreateThreadSafeLazy(Func<T> supplier)
        //{
        //    if (supplier == null)
        //    {
        //        throw new ArgumentNullException();
        //    }
        //
        //    return new ThreadSafeLazy<T>(supplier);
        //}
    }
}
