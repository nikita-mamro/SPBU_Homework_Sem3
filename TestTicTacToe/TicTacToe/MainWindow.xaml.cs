using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CellCondition[] fieldCondition;

        private bool isGameEnded;

        private List<Button> buttons;

        private bool playerATurn;

        public MainWindow()
        {
            InitializeComponent();

            buttons = new List<Button>();

            foreach (var child in Field.Children)
            {
                buttons.Add(child as Button);
            }

            StartGame();
        }
        /// <summary>
        /// Метод, который начинает новую партию
        /// </summary>
        private void StartGame()
        {
            fieldCondition = new CellCondition[9];

            for (var i = 0; i < 9; ++i)
            {
                fieldCondition[i] = CellCondition.Empty;
            }

            playerATurn = true;
            isGameEnded = false;

            foreach (var button in buttons)
            {
                button.Content = string.Empty;
                button.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Метод-обработчик нажатия на кнопку-ячейку поля
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isGameEnded)
            {
                StartGame();
                return;
            }

            var button = (Button)sender;

            var row = Grid.GetRow(button);
            var column = Grid.GetColumn(button);

            var position = column + 3 * row;

            MakeTurn(position, button);
        }

        /// <summary>
        /// Метод, отвечающий за совершение хода текущим игроком
        /// </summary>
        private void MakeTurn(int position, Button button)
        {
            if (fieldCondition[position] != CellCondition.Empty)
            {
                return;
            }

            button.Foreground = playerATurn ? Brushes.Blue : Brushes.Red;

            if (playerATurn)
            {
                fieldCondition[position] = CellCondition.X;
                button.Content = "X";
            }
            else
            {
                fieldCondition[position] = CellCondition.O;
                button.Content = "O";
            }

            playerATurn ^= true;

            CheckIfEnded(position);
        }

        /// <summary>
        /// Проверяет, закончилась ли партия (победа кого-либо / ничья)
        /// </summary>
        private void CheckIfEnded(int position)
        {
            var winCombination = WinChecker.TryToGetWinCombination(fieldCondition, position);

            if (winCombination != null)
            {
                buttons[winCombination[0]].Background = Brushes.Green;
                buttons[winCombination[1]].Background = Brushes.Green;
                buttons[winCombination[2]].Background = Brushes.Green;
                MessageBox.Show(this, "Победа");
                StartGame();
                return;
            }

            if (!fieldCondition.Contains(CellCondition.Empty))
            {
                MessageBox.Show(this, "Ничья");
                StartGame();
            }
        }
    }
}
