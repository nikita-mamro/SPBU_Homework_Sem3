using System;

namespace Lazy
{
    /// <summary>
    /// Класс, реализующий интерфейс ILazy,
    /// работающий корректно и в многопоточном режиме
    /// </summary>
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        /// <summary>
        /// Флаг, который говорит о том, вычислен ли результат
        /// </summary>
        private volatile bool isCounted = false;
        /// <summary>
        /// Объект, предоставляющий вычисление
        /// </summary>
        private Func<T> supplier;
        /// <summary>
        /// Хранящееся значение
        /// </summary>
        private T value;
        /// <summary>
        /// Объект, на который будем ставить замок
        /// </summary>
        private object lazyLock = new object();

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
                lock (lazyLock)
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
