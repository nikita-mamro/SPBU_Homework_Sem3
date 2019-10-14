using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool.Tests
{
    /// <summary>
    /// Тесты пула потоков
    /// </summary>
    [TestClass]
    public class MyThreadPoolTests
    {
        [TestMethod]
        public void SimpleCalculationsWithNotManyTasksTest()
        {
            var pool = new MyThreadPool(10);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 3; i++)
            {
                tasks.Add(pool.QueueTask(() => 2 * 2));

                Assert.AreEqual(4, tasks[i].Result);
                Assert.IsTrue(tasks[i].IsCompleted);
            }

            pool.Shutdown();
        }

        [TestMethod]
        public void SimpleCalculationsWithManyTasksTest()
        {
            var pool = new MyThreadPool(5);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 20; i++)
            {
                tasks.Add(pool.QueueTask(() => 2 * 2));

                Assert.AreEqual(4, tasks[i].Result);
                Assert.IsTrue(tasks[i].IsCompleted);
            }

            pool.Shutdown();
        }

        [TestMethod]
        public void LongCalculationsTest()
        {
            var pool = new MyThreadPool(5);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 20; i++)
            {
                tasks.Add(pool.QueueTask(() =>
                {
                    Thread.Sleep(100);
                    return 2 * 2;
                }));
            }

            Thread.Sleep(5000);

            foreach (var task in tasks)
            {
                Assert.AreEqual(4, task.Result);
                Assert.IsTrue(task.IsCompleted);
            }

            pool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(MyThreadPoolNotWorkingException))]
        public void AddNewTaskToNotWorkingPoolExceptionTest()
        {
            var pool = new MyThreadPool(1);
            pool.Shutdown();
            pool.QueueTask(() => 2 + 2);
        }

        [TestMethod]
        [ExpectedException(typeof(MyThreadPoolNotWorkingException))]
        public void ShutdownDeadPoolExceptionTest()
        {
            var pool = new MyThreadPool(1);
            pool.Shutdown();
            pool.Shutdown();
        }

        [TestMethod]
        public void OtherCalculationsIgnoreExceptionTest()
        {
            var pool = new MyThreadPool(5);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 5; ++i)
            {
                tasks.Add(pool.QueueTask(() => 2 * 2));
            }

            pool.QueueTask<int>(() =>
            {
                throw new SpecialTestException();
            });

            tasks.Add(pool.QueueTask(() => 2 * 2));

            foreach (var task in tasks)
            {
                Assert.AreEqual(4, task.Result);
                Assert.IsTrue(task.IsCompleted);
            }

            pool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(SpecialTestException))]
        public void CatchingExceptionFromTaskResultTest()
        {
            var pool = new MyThreadPool(5);

            var task = pool.QueueTask<int>(() =>
            {
                throw new SpecialTestException();
            });

            Thread.Sleep(10);
            pool.Shutdown();

            var res = task.Result;
        }

        [TestMethod]
        public void BasicContinueWithTest()
        {
            var pool = new MyThreadPool(5);

            var task = pool.QueueTask(() =>
            {
                Thread.Sleep(100);
                return 2 * 2;
            });

            var nextTask = task.ContinueWith((value) =>
            {
                Thread.Sleep(100);
                return value * 2;
            });

            // Ждём выполнения последовательных задач
            Thread.Sleep(250);

            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(nextTask.IsCompleted);
            Assert.AreEqual(4, task.Result);
            Assert.AreEqual(8, nextTask.Result);

            pool.Shutdown();
        }

        [TestMethod]
        public void MultipleContinueWithDifferentTypesTest()
        {
            var pool = new MyThreadPool(5);

            var task = pool.QueueTask(() =>
            {
                Thread.Sleep(100);
                return "Hello, next task";
            });

            var nextTask = task.ContinueWith((value) =>
            {
                Thread.Sleep(100);
                return value.Length;
            });

            var afterNextTask = nextTask.ContinueWith((value) =>
            {
                Thread.Sleep(100);
                return value.ToString();
            });

            var afterAfterNextTask = afterNextTask.ContinueWith((value) =>
            {
                Thread.Sleep(100);
                return value.Length;
            });

            // Ждём выполнения последовательных задач
            Thread.Sleep(450);

            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(nextTask.IsCompleted);
            Assert.IsTrue(afterNextTask.IsCompleted);
            Assert.AreEqual("Hello, next task", task.Result);
            Assert.AreEqual(16, nextTask.Result);
            Assert.AreEqual("16", afterNextTask.Result);
            Assert.AreEqual(2, afterAfterNextTask.Result);

            pool.Shutdown();
        }

        [TestMethod]
        public void OtherCalculationsIgnoreContinueWithExceptionTest()
        {
            var pool = new MyThreadPool(5);

            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 5; ++i)
            {
                tasks.Add(pool.QueueTask(() => 2 * 2));
            }

            tasks[4].ContinueWith<int>((value) =>
            {
                throw new SpecialTestException();
            });

            tasks.Add(pool.QueueTask(() => 2 * 2));

            foreach (var task in tasks)
            {
                Assert.AreEqual(4, task.Result);
                Assert.IsTrue(task.IsCompleted);
            }

            pool.Shutdown();
        }

        [TestMethod]
        [ExpectedException(typeof(SpecialTestException))]
        public void CatchingExceptionFromContinueWithResultTest()
        {
            var pool = new MyThreadPool(5);

            var task = pool.QueueTask<int>(() => 2 * 2);
            var nextTask = task.ContinueWith<int>((value) =>
            {
                throw new SpecialTestException();
            });

            Thread.Sleep(10);
            pool.Shutdown();

            var res = nextTask.Result;
        }

        [TestMethod]
        public void CalculatingAfterShutdownTest()
        {
            var pool = new MyThreadPool(1);

            var task = pool.QueueTask(() =>
            {
                Thread.Sleep(100);
                return 2 * 2;
            });

            pool.Shutdown();

            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void CalculatingFromQueueAfterShutdownTest()
        {
            var pool = new MyThreadPool(2);
            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < 10; ++i)
            {
                tasks.Add(pool.QueueTask(() =>
                {
                    Thread.Sleep(10);
                    return 2 * 2;
                }));
            }

            pool.Shutdown();

            foreach (var task in tasks)
            {
                Assert.IsTrue(task.IsCompleted);
                Assert.AreEqual(4, task.Result);
            }
        }
        
        [TestMethod]
        public void MultiThreadTaskResult()
        {
            var pool = new MyThreadPool(1);

            var task = pool.QueueTask(() =>
            {
                return 2 * 2;
            });

            Thread.Sleep(10);
            pool.Shutdown();

            int result1 = 0;
            int result2 = 0;

            var resetEvent = new ManualResetEvent(false);

            var thread1 = new Thread(() =>
            {
                resetEvent.WaitOne();
                result1 = task.Result;
            });

            var thread2 = new Thread(() =>
            {
                resetEvent.WaitOne();
                result2 = task.Result;
            });

            thread1.Start();
            thread2.Start();

            resetEvent.Set();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(4, result1);
            Assert.AreEqual(4, result2);
        }
    }
}