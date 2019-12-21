using MyNUnit.Attributes;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit.MyNUnitTools
{
    /// <summary>
    /// Класс, реализующий множество всех методов, необходимых для тестирования выбранного класса
    /// </summary>
    public class MethodsSet
    {
        /// <summary>
        /// Очередь из методов, помеченных аттрибутом BeforeClass
        /// </summary>
        /// <seealso cref="BeforeClassAttribute"/>
        public ConcurrentQueue<MethodInfo> BeforeClassTestMethods;

        /// <summary>
        /// Очередь из методов, помеченных аттрибутом Before
        /// </summary>
        /// <seealso cref="BeforeAttribute"/>
        public ConcurrentQueue<MethodInfo> BeforeTestMethods;

        /// <summary>
        /// Очередь из методов, помеченных аттрибутом Test
        /// </summary>
        /// <seealso cref="TestAttribute"/>
        public ConcurrentQueue<MethodInfo> TestMethods;

        /// <summary>
        /// Очередь из методов, помеченных аттрибутом After
        /// </summary>
        /// <seealso cref="AfterAttribute"/>
        public ConcurrentQueue<MethodInfo> AfterTestMethods;

        /// <summary>
        /// Очередь из методов, помеченных аттрибутом AfterClass
        /// </summary>
        /// <seealso cref="AfterClassAttribute"/>
        public ConcurrentQueue<MethodInfo> AfterClassTestMethods;

        /// <summary>
        /// Количество тестируемых методов
        /// </summary>
        public int TestsCount
                => TestMethods.Count;

        public MethodsSet(Type type)
        {
            BeforeClassTestMethods = new ConcurrentQueue<MethodInfo>();
            BeforeTestMethods = new ConcurrentQueue<MethodInfo>();
            TestMethods = new ConcurrentQueue<MethodInfo>();
            AfterTestMethods = new ConcurrentQueue<MethodInfo>();
            AfterClassTestMethods = new ConcurrentQueue<MethodInfo>();

            QueueTestedClassMethods(type);
        }

        /// <summary>
        /// Проверка на то, подходит ли метод для тестирования
        /// </summary>
        private bool IsMethodValid(MethodInfo method)
        {
            return method.GetParameters().Length == 0 && method.ReturnType == typeof(void);
        }

        /// <summary>
        /// Заполняет очереди методов для тестирования для переданного класса
        /// </summary>
        public void QueueTestedClassMethods(Type type)
        {
            Parallel.ForEach(type.GetMethods(), method =>
            {
                if (method.GetCustomAttribute<BeforeClassAttribute>() != null)
                {
                    if (!method.IsStatic)
                    {
                        throw new FormatException("BeforeClass methods must be static");
                    }

                    TryToQueueMethod(method, BeforeClassTestMethods);
                }
                else if (method.GetCustomAttribute<BeforeAttribute>() != null)
                {
                    TryToQueueMethod(method, BeforeTestMethods);
                }
                else if (method.GetCustomAttribute<TestAttribute>() != null)
                {
                    TryToQueueMethod(method, TestMethods);
                }
                else if (method.GetCustomAttribute<AfterAttribute>() != null)
                {
                    TryToQueueMethod(method, AfterTestMethods);
                }
                else if (method.GetCustomAttribute<AfterClassAttribute>() != null)
                {
                    if (!method.IsStatic)
                    {
                        throw new FormatException("AfterClass methods must be static");
                    }

                    TryToQueueMethod(method, AfterClassTestMethods);
                }
            });
        }

        /// <summary>
        /// Попытка добавить метод в очередь
        /// </summary>
        private void TryToQueueMethod(MethodInfo method, ConcurrentQueue<MethodInfo> queue)
        {
            if (!IsMethodValid(method))
            {
                throw new FormatException("Method shouldn't return value or get parameters");
            }

            queue.Enqueue(method);
        }
    }
}
