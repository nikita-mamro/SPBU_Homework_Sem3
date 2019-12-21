using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Concurrent;
using MyNUnit.Attributes;
using MyNUnit.MyNUnitTools;
using System.Diagnostics;

namespace MyNUnit
{
    public static class MyNUnit
    {
        /// <summary>
        /// Методы для тестирования
        /// </summary>
        private static ConcurrentDictionary<Type, MethodsSet> methodsToTest = new ConcurrentDictionary<Type, MethodsSet>();

        /// <summary>
        /// Результаты тестов
        /// </summary>
        private static ConcurrentDictionary<Type, ConcurrentBag<TestInfo>> testResults = new ConcurrentDictionary<Type, ConcurrentBag<TestInfo>>();

        /// <summary>
        /// Запуск тестов с выводом результатов на экран
        /// </summary>
        public static void RunTests(string path)
        {
            ProceedPathAndExecuteTests(path);

            PrintReport();
        }

        /// <summary>
        /// Запуск тестов с сохранением результатов в виде словаря
        /// </summary>
        public static Dictionary<Type, List<TestInfo>> RunTestsAndGetReport(string path)
        {
            ProceedPathAndExecuteTests(path);

            return GetDictionaryOfReports();
        }

        private static void ProceedPathAndExecuteTests(string path)
        {
            var classes = GetClasses(path);

            Parallel.ForEach(classes, someClass =>
            {
                QueueClassTests(someClass);
            });

            ExecuteAllTests();
        }

        private static Dictionary<Type, List<TestInfo>> GetDictionaryOfReports()
        {
            var res = new Dictionary<Type, List<TestInfo>>();

            foreach (var type in testResults.Keys)
            {
                res.Add(type, new List<TestInfo>());

                foreach (var testInfo in testResults[type])
                {
                    res[type].Add(testInfo);
                }
            }

            return res;
        }

        /// <summary>
        /// Получение сборок по заданному пути
        /// </summary>
        private static List<string> GetAssemblyPaths(string path)
        {
            var res = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(path, "*.exe", SearchOption.AllDirectories)).ToList();
            res.RemoveAll(assemblyPath => assemblyPath.Contains("\\MyNUnit.exe"));
            return res;
        }

        /// <summary>
        /// Получение классов из сборок по заданному пути
        /// </summary>
        private static IEnumerable<Type> GetClasses(string path)
        {
            return GetAssemblyPaths(path).Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass);
        }

        /// <summary>
        /// Загрузка методов для тестирования переданного класса в очередь
        /// </summary>
        private static void QueueClassTests(Type type)
        {
            methodsToTest.TryAdd(type, new MethodsSet(type));
        }

        /// <summary>
        /// Исполнение всех тестов
        /// </summary>
        private static void ExecuteAllTests()
        {
            Parallel.ForEach(methodsToTest.Keys, type =>
            {
                testResults.TryAdd(type, new ConcurrentBag<TestInfo>());

                foreach (var beforeClassMethod in methodsToTest[type].BeforeClassTestMethods)
                {
                    ExecuteNonTestMethod(beforeClassMethod, null);
                }

                foreach (var testMethod in methodsToTest[type].TestMethods)
                {
                    ExecuteTestMethod(type, testMethod);
                }

                foreach (var afterClassMethod in methodsToTest[type].AfterClassTestMethods)
                {
                    ExecuteNonTestMethod(afterClassMethod, null);
                }
            });
        }

        /// <summary>
        /// Исполение переданного тестового метода
        /// </summary>
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
                ExecuteNonTestMethod(beforeTestMethod, instance);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                method.Invoke(instance, null);

                if (attribute.ExpectedException == null)
                {
                    isPassed = true;
                    stopwatch.Stop();
                }
            }
            catch (Exception testException)
            {
                thrownException = testException.InnerException.GetType();

                if (thrownException == attribute.ExpectedException)
                {
                    isPassed = true;
                    stopwatch.Stop();
                }
            }
            finally
            {
                stopwatch.Stop();
                var ellapsedTime = stopwatch.Elapsed;
                testResults[type].Add(new TestInfo(method.Name, isPassed, attribute.ExpectedException, thrownException, ellapsedTime));
            }

            foreach (var afterTestMethod in methodsToTest[type].AfterTestMethods)
            {
                ExecuteNonTestMethod(afterTestMethod, instance);
            }
        }

        /// <summary>
        /// Исполнение метода, который требуется для тестирования, но не помеченного аттрибутом [Test]
        /// </summary>
        private static void ExecuteNonTestMethod(MethodInfo method, object instance)
        {
            method.Invoke(instance, null);
        }
        
        /// <summary>
        /// Распечатывает результаты тестирования
        /// </summary>
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

            Console.WriteLine($"Found methods to test (total): {allMethodsCount}");

            foreach (var someClass in testResults.Keys)
            {
                Console.WriteLine("-----------------------------");
                Console.WriteLine($"Class: {someClass.Name}");

                var test = testResults;

                foreach (var testInfo in testResults[someClass])
                {
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine($"Tested method: {testInfo.MethodName}()");

                    if (testInfo.IsIgnored == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Ignored {testInfo.MethodName}() with message: {testInfo.IgnoranceMessage}");
                        Console.ResetColor();
                        continue;
                    }

                    if (testInfo.ExpectedException != null || testInfo.TestException != null)
                    {
                        if (testInfo.ExpectedException == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unexpected exception: {testInfo.TestException}");
                            Console.ResetColor();
                        }
                        else
                        {
                            if (testInfo.ExpectedException == testInfo.TestException)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }

                            Console.WriteLine($"Expected exception: {testInfo.ExpectedException}");
                            Console.WriteLine($"Thrown exception: {testInfo.TestException}");

                            Console.ResetColor();
                        }
                    }

                    Console.WriteLine($"Ellapsed time: {testInfo.Time}");

                    if (testInfo.IsPassed)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Passed {testInfo.MethodName}() test");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Failed {testInfo.MethodName}() test");
                        Console.ResetColor();
                    }
                }
            }

            Console.WriteLine("-----------------------------");
        }
    }
}
