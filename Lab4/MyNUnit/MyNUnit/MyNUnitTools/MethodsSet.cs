using MyNUnit.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.MyNUnitTools
{
    public class MethodsSet
    {
        public ConcurrentQueue<MethodInfo> BeforeClassTestMethods;
        public ConcurrentQueue<MethodInfo> BeforeTestMethods;
        public ConcurrentQueue<MethodInfo> TestMethods;
        public ConcurrentQueue<MethodInfo> AfterTestMethods;
        public ConcurrentQueue<MethodInfo> AfterClassTestMethods;

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

        private bool IsMethodValid(MethodInfo method)
        {
            return method.GetParameters().Length == 0 && method.ReturnType == typeof(void);
        }

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

        private void TryToQueueMethod(MethodInfo method, ConcurrentQueue<MethodInfo> queue)
        {
            if (!IsMethodValid(method))
            {
                throw new InvalidOperationException("Method shouldn't return value or get parameters");
            }

            queue.Enqueue(method);
        }
    }
}
