using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Этим аттрибутом помечаются тестовые методы
    /// </summary>
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Тип ожидаемого исключения
        /// </summary>
        public Type ExpectedException { get; private set; }

        /// <summary>
        /// Сообщение, объясняющее, почему метод игнорируется при тестировании
        /// </summary>
        public string IgnoreMessage { get; private set; }

        /// <summary>
        /// Хранит информацию о том, игнорируется ли метод при тестировании
        /// </summary>
        public bool IsIgnored
            => IgnoreMessage != "";

        public TestAttribute(string ignore = "", Type expected = null)
        {
            ExpectedException = expected;
            IgnoreMessage = ignore;
        }
    }
}
