﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Класс, реализующий модель, содержащую информацию о результатах тестирования
    /// </summary>
    public class TestInfo
    {
        /// <summary>
        /// Имя протестированного метода
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Индикатор того, прошёл ли тест успешно
        /// </summary>
        public bool IsPassed { get; private set; }

        /// <summary>
        /// Тип ожидаемого исключения
        /// </summary>
        public Type ExpectedException { get; private set; }

        /// <summary>
        /// Исключение, брошенное тестируемым методом
        /// </summary>
        public Type TestException { get; private set; }

        /// <summary>
        /// Индикатор того, игнорировался ли метод при тестировании
        /// </summary>
        public bool IsIgnored { get; private set; } = false;

        /// <summary>
        /// Сообщение с информацией о причине игнорирования
        /// </summary>
        public string IgnoranceMessage { get; private set; } = "";

        /// <summary>
        /// Конструктор для создания отчёта о проигнорированном методе
        /// </summary>
        public TestInfo(string name, bool isIgnored, string ignoranceMessage)
        {
            MethodName = name;
            IsIgnored = true;
            IgnoranceMessage = ignoranceMessage;
        }

        /// <summary>
        /// Конструктор для создания отчёта о протестирванном методе
        /// </summary>
        public TestInfo(string name, bool isPassed, Type expectedException, Type thrownException) 
        {
            MethodName = name;
            IsPassed = isPassed;
            ExpectedException = expectedException;
            TestException = thrownException;
        }
    }
}
