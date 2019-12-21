using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Этим аттрибутом помечаются методы, 
    /// которые должны будут исполняться после выполнения всех остальных тестовых методов в классе
    /// </summary>
    public class AfterClassAttribute : Attribute { }
}
