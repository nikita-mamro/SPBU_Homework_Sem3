using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            DataContext = model;

            InitializeComponent();

            filesAndFoldersServerListView.ItemsSource = model.DisplayedListOnServer;
            clientListView.ItemsSource = model.DisplayedListOnClient;
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            MessageBox.Show(Application.Current.MainWindow,
                "To open folder: DoubleClick on folder\n" +
                "To connect to server and see its explorer: Input address and port and click on Connect button\n" +
                "To set download fodler: Click Choose button when in the folder you want to set as download folder\n" +
                "To download file from server: Choose download folder and double click on file\n", "Hint");
        }

        private void PortTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void HandleServerDoubleClick(object sender, RoutedEventArgs e)
        {
             await model.OpenServerFolderOrDownloadFile((sender as ListViewItem).Content.ToString());
        }

        private void HandleClientDoubleClick(object sender, RoutedEventArgs e)
        {
            model.OpenClientFolder((sender as ListViewItem).Content.ToString());
        }

        private void ShowMessage(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error occured");
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
             await model.Connect();
        }

        private async void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
             await model.DownloadAllFilesInCurrentDirectory();
        }

        private async void BackServer_Click(object sender, RoutedEventArgs e)
        {
             await model.GoBackServer();
        }

        private void BackClient_Click(object sender, RoutedEventArgs e)
        {
            model.GoBackClient();
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            model.UpdateDownloadFolder();
        }

        private void addressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.IsConnected = false;
        }

        private void portTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.IsConnected = false;
        }
    }
}
