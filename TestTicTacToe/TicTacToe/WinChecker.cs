using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    /// <summary>
    /// Класс для проверки ситуации на поле
    /// </summary>
    public static class WinChecker
    {
        /// <summary>
        /// Проверяет, есть ли выигрышная комбинация относительно переданной позиции
        /// </summary>
        /// <param name="fieldCondition">Состояние поля на момент проверки</param>
        /// <returns>Победные позиции / null, если таких нет</returns>
        public static List<int> TryToGetWinCombination(CellCondition[] fieldCondition, int position)
        {
            var currentCellCondition = fieldCondition[position];

            var toCheck = CombinationsToCheck(position);

            foreach (var combination in toCheck)
            {
                if (fieldCondition[combination[0]] == currentCellCondition
                    && fieldCondition[combination[1]] == currentCellCondition)
                {
                    return new List<int>() { position, combination[0], combination[1]};
                }
            }

            return null;
        }

        /// <summary>
        /// Получает соседей для переданной клетки, требующих проверки
        /// </summary>
        private static List<List<int>> CombinationsToCheck(int position)
        {
            var row = position / 3;

            var res = new List<List<int>>();

            var horizontal = new List<int>() { (position + 1) % 3 + row * 3, (position + 2) % 3 + row * 3 };

            var vertical = new List<int>() { (position + 3) % 9, (position + 6) % 9 };

            res.Add(horizontal);
            res.Add(vertical);

            if (position % 2 == 0)
            {
                if (position == 4)
                {
                    res.Add(new List<int>() { 0, 8 });
                    res.Add(new List<int>() { 2, 6 });
                    return res;
                }

                var diag048 = new List<int>() { (position + 4) % 12, (position + 8) % 12 };
                var diag246 = new List<int>() { (position) % 6 + 2, (position) % 6 + 4};

                if (position % 4 == 0)
                {
                    res.Add(diag048);
                    return res;
                }

                res.Add(diag246);
            }

            return res;
        }
    }
}
