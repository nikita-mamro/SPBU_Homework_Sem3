using System;

namespace MyThreadPool.Tests
{
    /// <summary>
    /// Исключение, которое бросаем только в тестах
    /// Чтобы наверняка быть уверенными, что кидается именно ожидаемое исключение
    /// </summary>
    [Serializable]
    public class SpecialTestException : Exception
    {
        public SpecialTestException() : base() { }
    }
}
