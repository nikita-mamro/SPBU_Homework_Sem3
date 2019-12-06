using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using TicTacToe;

namespace TicTacToeTests
{
    /// <summary>
    /// Пара тестов, которые успел
    /// </summary>
    [TestClass]
    public class WinCheckerTests
    {
        CellCondition[] field = new CellCondition[9];

        List<int> leftTopDiag = new List<int> { 0, 4, 8 };
        List<int> leftBottomDiag = new List<int> { 2, 4, 6 };

        [TestMethod]
        public void TestChecker048X()
        {
            field[leftTopDiag[0]] = CellCondition.X;
            field[leftTopDiag[1]] = CellCondition.X;
            field[leftTopDiag[2]] = CellCondition.X;

            var res = WinChecker.TryToGetWinCombination(field, leftTopDiag[0]);

            Assert.IsTrue(res != null);

            res = WinChecker.TryToGetWinCombination(field, leftTopDiag[1]);

            Assert.IsTrue(res != null);

            res = WinChecker.TryToGetWinCombination(field, leftTopDiag[2]);

            Assert.IsTrue(res != null);
        }

        [TestMethod]
        public void TestChecker048O()
        {
            field[leftTopDiag[0]] = CellCondition.O;
            field[leftTopDiag[1]] = CellCondition.O;
            field[leftTopDiag[2]] = CellCondition.O;

            var res = WinChecker.TryToGetWinCombination(field, leftTopDiag[0]);

            Assert.IsTrue(res != null);

            res = WinChecker.TryToGetWinCombination(field, leftTopDiag[1]);

            Assert.IsTrue(res != null);

            res = WinChecker.TryToGetWinCombination(field, leftTopDiag[2]);

            Assert.IsTrue(res != null);
        }
    }
}
