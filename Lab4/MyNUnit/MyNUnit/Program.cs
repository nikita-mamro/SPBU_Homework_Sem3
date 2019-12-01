using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //var assemblies = getAss

            foreach (Assembly a in assemblies)
            {
                Console.WriteLine($"Assembly: {a}");
                foreach (Type t in a.ExportedTypes)
                {
                    foreach (MethodInfo mi in t.GetMethods())
                    {
                        foreach (Attribute attr in Attribute.GetCustomAttributes(mi))
                        {
                            if (attr.GetType() == typeof(Attributes.TestAttribute))
                            {
                                Console.WriteLine("Method {0} has a {1} attribute.", mi.Name, ((Attributes.TestAttribute)attr));
                            }
                        }
                    }
                }
            }
        }
    }
}
