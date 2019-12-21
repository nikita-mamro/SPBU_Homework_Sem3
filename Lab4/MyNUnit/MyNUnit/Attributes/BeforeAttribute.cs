using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Этим аттрибутом помечаются методы, 
    /// которые должны будут исполняться перед каждым тестовым методом
    /// </summary>
    public class BeforeAttribute : Attribute { }
}
