using System;

namespace Lazy
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "test";

            Func<string> supplier = () => str;

            var lazyTest = LazyFactory<string>.CreateLazy(supplier);

            Console.WriteLine(lazyTest.Get());
        }
    }
}
