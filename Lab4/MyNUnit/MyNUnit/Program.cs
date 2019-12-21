using System;
using System.IO;
using System.Linq;

namespace MyNUnit
{
    class Program
    {
        /// <summary>
        /// Пример работы MyNUnit с выводом результатов на экран
        /// </summary>
        static void Main(string[] args)
        {
            var root = "..\\..\\..\\TestProjects\\TestResult\\Assembly";

            MyNUnit.RunTests(root);
        }

        static void PrintAllFolders()
        {
            var root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

            var res = Directory.EnumerateFiles(root, "*.dll", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(root, "*.exe", SearchOption.AllDirectories));

            foreach (var f in res)
            {
                Console.WriteLine(f);
            }
        }
    }
}
