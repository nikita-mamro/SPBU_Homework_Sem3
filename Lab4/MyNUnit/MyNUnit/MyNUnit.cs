using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNUnit.Attributes;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;

namespace MyNUnit
{
    public static class MyNUnit
    {
        private static ConcurrentDictionary<Type, ConcurrentQueue<MethodInfo>> methodsToTest;

        public static void RunTests(string path)
        {
            var classes = GetClasses(path);

            foreach (var e in classes)
            {
                QueueClassTests(e);
            }

            ExecuteAllTests();
        }

        private static IEnumerable<Type> GetClasses(string path)
        {
            var files  = Directory.EnumerateFiles(path, "*.dll");
            var classes = files.Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass);

            return classes;
        }

        private static void QueueClassTests(Type t)
        {
            foreach (var method in t.GetMethods())
            {
                foreach (var attribute in method.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(TestAttribute))
                    {
                        methodsToTest[t].Enqueue(method);
                    }
                }
            }
        }

        private static void ExecuteAllTests()
        {
            Parallel.ForEach(methodsToTest.Keys, key =>
            {
                Parallel.ForEach(methodsToTest[key], method =>
                {
                    ExecuteTest(method);
                });
            });
        }

        private static void ExecuteTest(MethodInfo method)
        {

        }
    }
}
