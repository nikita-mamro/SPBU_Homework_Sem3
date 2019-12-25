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
        /// <summary>
        /// Модель,
        /// </summary>
        private ClientViewModel model;

        public MainWindow()
        {
            model = new ClientViewModel("..\\..\\..\\Client\\res\\Downloads\\");
            model.ErrorHandler += ShowMessage;
            DataContext = model;

            InitializeComponent();

            filesAndFoldersServerListView.ItemsSource = model.DisplayedListOnServer;
            clientListView.ItemsSource = model.DisplayedListOnClient;
            filesToDownloadListView.ItemsSource = model.DownloadsInProcessList;
            downloadedListView.ItemsSource = model.DownloadsFinishedList;
        }

        /// <summary>
        /// Открытие окна с инструкциями после рендера главного окна
        /// </summary>
        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            MessageBox.Show(Application.Current.MainWindow,
                "To open folder: DoubleClick on folder\n" +
                "To connect to server and see its explorer: Input address and port and click on Connect button\n" +
                "To set download fodler: Click Choose button when in the folder you want to set as download folder\n" +
                "To download file from server: Choose download folder and double click on file\n", "Hint");
        }

        /// <summary>
        /// Валидатор, не дающий вводить не цифры в поле ввода порта
        /// </summary>
        private void PortTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Обработчик двойного нажатия на элемент менеджера файлов и папок на сервере
        /// </summary>
        private async void HandleServerDoubleClick(object sender, RoutedEventArgs e)
        {
             await model.OpenServerFolderOrDownloadFile((sender as ListViewItem).Content.ToString());
        }

        /// <summary>
        /// Обработчик двойного нажатия на элемент менеджера папок в клиенте
        /// </summary>
        private void HandleClientDoubleClick(object sender, RoutedEventArgs e)
        {
            model.OpenClientFolder((sender as ListViewItem).Content.ToString());
        }

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        private void ShowMessage(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error occured");
        }

        /// <summary>
        /// Обработчик нажатия на кнопку подключения
        /// </summary>
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
             await model.Connect();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку загрузки всех файлов
        /// </summary>
        private async void DownloadAll_Click(object sender, RoutedEventArgs e)
        {
             await model.DownloadAllFilesInCurrentDirectory();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "назад" на сервере
        /// </summary>
        private async void BackServer_Click(object sender, RoutedEventArgs e)
        {
             await model.GoBackServer();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "назад" в клиенте
        /// </summary>
        private void BackClient_Click(object sender, RoutedEventArgs e)
        {
            model.GoBackClient();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку для сохранения текущей папки для загрузки
        /// </summary>
        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            model.UpdateDownloadFolder();
        }

        /// <summary>
        /// Обработчик изменения значения адреса для подключения
        /// </summary>
        private void addressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.IsConnected = false;
        }

        /// <summary>
        /// Обработчик изменения значения порта для подключения
        /// </summary>
        private void portTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            model.IsConnected = false;
        }
    }
}
