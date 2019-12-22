using System.Windows;
using System.Windows.Controls;
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
            model = new ClientViewModel();
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

        private async void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            await model.DownloadAllFilesInCurrentDirectory();
        }
    }
}
