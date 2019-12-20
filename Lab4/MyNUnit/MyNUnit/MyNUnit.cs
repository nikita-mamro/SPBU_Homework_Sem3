using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using MyNUnit.Attributes;

namespace MyNUnit
{
    public static class MyNUnit
    {
        private static ConcurrentDictionary<Type, ConcurrentQueue<MethodInfo>> methodsToTest = new ConcurrentDictionary<Type, ConcurrentQueue<MethodInfo>>();

        public static void RunTests(string path)
        {
            // Working
            var classes = WorkingGetClasses(path);

            // Not working
            foreach (var someClass in classes)
            {
                QueueClassTests(someClass);
            }

            // Not working
            //var classes = GetClasses(path);
            //
            //foreach (var e in classes)
            //{
            //    QueueClassTests(e);
            //}
        }

        private static IEnumerable<string> GetAssemblyPaths(string path)
        {
            return Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(path, "*.exe", SearchOption.AllDirectories));
        }

        private static IEnumerable<Type> GetClasses(string path)
        {
            // Not working for some reason
            return GetAssemblyPaths(path).Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass);
        }

        private static List<Type> WorkingGetClasses(string path)
        {
            // Working pretty well
            var res = new List<Type>();

            foreach (var assemblyPath in GetAssemblyPaths(path))
            {
                foreach (var type in Assembly.LoadFrom(assemblyPath).ExportedTypes)
                {
                    if (type.IsClass)
                    {
                        Console.WriteLine(type.Name);
                        res.Add(type);
                    }
                }
            }

            return res;
        }

        private static void QueueClassTests(Type t)
        {
            foreach (var methodInfo in t.GetMethods())
            {
                if (methodInfo.GetCustomAttributes<TestAttribute>().Count() != 0)
                {
                    Console.WriteLine(methodInfo.Name); // No output from here
                    methodsToTest[t].Enqueue(methodInfo);
                }
            }
        }

        private static void ExecuteAllTests()
        {
            Parallel.ForEach(methodsToTest.Keys, key =>
            {
                Parallel.ForEach(methodsToTest[key], method =>
                {
                    ExecuteTestMethod(key, method);
                });
            });
        }

        private static void ExecuteNonTestMethods()
        {

        }

        private static void ExecuteTestMethod(Type type, MethodInfo method)
        {
        }
        
    }
}
