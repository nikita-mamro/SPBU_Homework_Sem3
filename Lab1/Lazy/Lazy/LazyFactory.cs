using System;

namespace Lazy
{
    /// <summary>
    /// Реализация абстрактной фабрики для Lazy<T>
    /// </summary>
    /// <typeparam name="T">Тип значения, которое хранится в Lazy</typeparam>
    public static class LazyFactory<T>
    {
        /// <summary>
        /// Создание экземпляра небезопасного Lazy
        /// </summary>
        /// <param name="supplier">Объект, предоставляющий вычисление</param>
        public static Lazy<T> CreateLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException();
            }

            return new Lazy<T>(supplier);
        }

        /// <summary>
        /// Создание экземпляра небезопасного Lazy
        /// </summary>
        /// <param name="supplier">Объект, предоставляющий вычисление</param>
        public static ThreadSafeLazy<T> CreateThreadSafeLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException();
            }
        
            return new ThreadSafeLazy<T>(supplier);
        }
    }
}
