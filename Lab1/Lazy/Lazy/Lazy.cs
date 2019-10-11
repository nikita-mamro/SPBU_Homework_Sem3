using System;

namespace Lazy
{
    /// <summary>
    /// Класс, реализующий интерфейс ILazy,
    /// не гарантирована корректная работа в многопоточном режиме
    /// </summary>
    public class Lazy<T> : ILazy<T>
    {
        /// <summary>
        /// Флаг, который говорит о том, вычислен ли результат
        /// </summary>
        private bool isCounted = false;

        /// <summary>
        /// Объект, предоставляющий вычисление
        /// </summary>
        private Func<T> supplier;

        /// <summary>
        /// Хранящееся значение
        /// </summary>
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
                supplier = null;
                isCounted = true;
            }

            return value;
        }
    }
}
