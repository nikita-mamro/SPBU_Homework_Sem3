using System;

namespace MyNUnit.Attributes
{
    /// <summary>
    /// Этим аттрибутом помечаются методы, 
    /// которые должны будут исполняться перед выполнением всех остальных тестовых методов в классе
    /// </summary>
    public class BeforeClassAttribute : Attribute { } 
}
