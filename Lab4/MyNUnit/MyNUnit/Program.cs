﻿using System;
using System.IO;

namespace MyNUnit
{
    class Program
    {
        /// <summary>
        /// Пример работы MyNUnit с выводом результатов на экран
        /// </summary>
        static void Main(string[] args)
        {
            var root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            
            root = Path.Combine(root, "TestProjects\\TestResult\\bin");
            
            MyNUnit.RunTests(root);
        }
    }
}
