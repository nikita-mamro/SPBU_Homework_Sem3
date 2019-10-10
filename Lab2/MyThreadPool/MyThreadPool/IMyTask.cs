using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// Интерфейс задач, переданных на исполнение в MyThreadPool
    /// </summary>
    /// <typeparam name="TResult">Тип результата выполнения задачи</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// True, если задача выполнена, иначе false
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Результат выполнения задачи
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Принимает объект типа Func<TResult, TNewResult>, 
        /// который может быть применен к результату данной задачи,
        /// и возвращает новую задачу, принятую к исполнению
        /// </summary>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> supplier);
    }
}
