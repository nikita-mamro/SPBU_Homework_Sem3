using System;
using System.IO;
using System.Linq;

namespace MyNUnit
{
    class Program
    {
        /// <summary>
        /// Запуск тестов для переданной сборки, путь к которой был передан аргументом
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You should pass arguments to command line");
                return;
            }

            var path = args[0];

            try
            {
                MyNUnit.RunTests(path);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory not found, check if the input is correct.");
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to run tests, check the input and try again.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
