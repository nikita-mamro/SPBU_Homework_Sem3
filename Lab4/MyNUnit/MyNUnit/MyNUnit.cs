using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using MyNUnit.Attributes;
using MyNUnit.MyNUnitTools;

namespace MyNUnit
{
    public static class MyNUnit
    {
        private static ConcurrentDictionary<Type, MethodsSet> methodsToTest = new ConcurrentDictionary<Type, MethodsSet>();
        private static ConcurrentDictionary<Type, ConcurrentBag<TestInfo>> testResults = new ConcurrentDictionary<Type, ConcurrentBag<TestInfo>>();

        public static void RunTests(string path)
        {
            var classes = GetClasses(path);

            Parallel.ForEach(classes, someClass =>
            {
                QueueClassTests(someClass);
            });

            ExecuteAllTests();

            PrintReport();
        }

        private static List<string> GetAssemblyPaths(string path)
        {
            var res =  Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(path, "*.exe", SearchOption.AllDirectories)).ToList();
            res.RemoveAll(assemblyPath => assemblyPath.Contains("\\MyNUnit.exe"));
            return res;
        }

        private static IEnumerable<Type> GetClasses(string path)
        {
            return GetAssemblyPaths(path).Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass);
        }

        private static void QueueClassTests(Type type)
        {
            methodsToTest.TryAdd(type, new MethodsSet(type));
        }

        private static void ExecuteAllTests()
        {
            Parallel.ForEach(methodsToTest.Keys, type =>
            {
                testResults.TryAdd(type, new ConcurrentBag<TestInfo>());

                foreach (var beforeClassMethod in methodsToTest[type].BeforeClassTestMethods)
                {
                    ExecuteNonTestMethod(type, beforeClassMethod, null);
                }

                foreach (var testMethod in methodsToTest[type].TestMethods)
                {
                    ExecuteTestMethod(type, testMethod);
                }

                foreach (var afterClassMethod in methodsToTest[type].AfterClassTestMethods)
                {
                    ExecuteNonTestMethod(type, afterClassMethod, null);
                }
            });
        }

        private static void ExecuteTestMethod(Type type, MethodInfo method)
        {
            var attribute = method.GetCustomAttribute<TestAttribute>();
            var isPassed = false;
            Type thrownException = null;

            var emptyConstructor = type.GetConstructor(Type.EmptyTypes);

            if (emptyConstructor == null)
            {
                throw new FormatException($"{type.Name} must have parameterless constructor");
            }

            var instance = emptyConstructor.Invoke(null);

            if (attribute.IsIgnored)
            {
                isPassed = true;
                testResults[type].Add(new TestInfo(method.Name, true, attribute.IgnoranceMessage));
                return;
            }

            foreach (var beforeTestMethod in methodsToTest[type].BeforeTestMethods)
            {
                ExecuteNonTestMethod(type, beforeTestMethod, instance);
            }

            try
            {
                method.Invoke(instance, null);

                if (attribute.ExpectedException == null)
                {
                    isPassed = true;
                }
            }
            catch (Exception testException)
            {
                thrownException = testException.InnerException.GetType();

                if (thrownException == attribute.ExpectedException)
                {
                    isPassed = true;
                }
            }
            finally
            {
                testResults[type].Add(new TestInfo(method.Name, isPassed, attribute.ExpectedException, thrownException));
            }

            foreach (var afterTestMethod in methodsToTest[type].AfterClassTestMethods)
            {
                ExecuteNonTestMethod(type, afterTestMethod, instance);
            }
        }

        private static void ExecuteNonTestMethod(Type type, MethodInfo method, object instance)
        {
            method.Invoke(instance, null);
        }
        
        private static void PrintReport()
        {
            Console.WriteLine("Testing report:");
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"Found classes to test: {methodsToTest.Keys.Count}");

            var allMethodsCount = 0;

            foreach (var testedClass in methodsToTest.Keys)
            {
                allMethodsCount += methodsToTest[testedClass].TestsCount;
            }

            var lol = methodsToTest;

            Console.WriteLine($"Found methods to test: {allMethodsCount}");

            foreach (var someClass in testResults.Keys)
            {
                Console.WriteLine("-----------------------------");
                Console.WriteLine($"Class: {someClass.Name}");

                var test = testResults;

                foreach (var testInfo in testResults[someClass])
                {
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine($"Tested method: {testInfo.MethodName}");

                    if (testInfo.IsIgnored == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Ignored with message: {testInfo.IgnoranceMessage}");
                        Console.ResetColor();
                        continue;
                    }

                    if (testInfo.ExpectedException != null || testInfo.TestException != null)
                    {
                        if (testInfo.ExpectedException == null)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"Unexpected exception: {testInfo.TestException}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"Expected exception: {testInfo.ExpectedException}");
                            Console.WriteLine($"Thrown exception: {testInfo.TestException}");
                        }
                    }

                    if (testInfo.IsPassed)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Passed {testInfo.MethodName} test");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed {testInfo.MethodName} test");
                        Console.ResetColor();
                    }
                }
            }

            Console.WriteLine("-----------------------------");
        }
    }
}
