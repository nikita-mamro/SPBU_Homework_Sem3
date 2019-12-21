using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MyNUnit;
using System.IO;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            
            root = Path.Combine(root, "MyNUnit\\TestProjects\\TestResult\\bin");
            
            MyNUnit.RunTests(root);
        }
    }
}
