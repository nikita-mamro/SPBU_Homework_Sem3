using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Этим аттрибутом помечаются методы, 
    /// которые должны будут исполняться после выполнения всех остальных тестовых методов в классе
    /// </summary>
    public class AfterClassAttribute : Attribute { }
}
