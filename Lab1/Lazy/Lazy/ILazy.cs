namespace Lazy
{
    /// <summary>
    /// Интерфейс, представляющий ленивое вычисление
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого объекта</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Вызывет вычисление и возвращает объект,
        /// при повторных вызовах возвращает объект
        /// </summary>
        T Get();
    }
}
