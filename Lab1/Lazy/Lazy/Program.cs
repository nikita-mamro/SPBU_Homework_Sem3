using System;
using System.Threading;

namespace Lazy
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "test";

            Func<string> supplier = () => str;

            ThreadSafeLazy<string> lazy = LazyFactory<string>.CreateThreadSafeLazy(supplier);

            var threadA = new Thread(() =>
            {
                Console.WriteLine(lazy.Get());
            });

            var threadB = new Thread(() =>
            {
                Console.WriteLine(lazy.Get());
            });

            threadA.Start();
            threadB.Start();

            Console.WriteLine($"Main thread: {lazy.Get()}");

            threadA.Join();
            threadB.Join();
        }
    }
}
