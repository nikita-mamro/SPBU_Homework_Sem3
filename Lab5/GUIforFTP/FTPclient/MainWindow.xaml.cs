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
using ViewModel;

namespace FTPclient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientViewModel model;

        public MainWindow()
        {
            model = new ClientViewModel("127.0.0.1", 8888);
            model.ErrorHandler += ShowMessage;

            InitializeComponent();

            AddressPortGrid.DataContext = model;

            filesAndFoldersListView.ItemsSource = model.DisplayedList;
        }

        private async void HandleDoubleClick(object sender, RoutedEventArgs e)
        {
            await model.OpenFolderOrDownloadFile((sender as ListViewItem).Content.ToString());
        }

        private void ShowMessage(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage);
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            await model.Connect();
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
        {
            await model.GoBack();
        }
    }
}
