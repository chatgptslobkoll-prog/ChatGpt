using System.Windows;
using Microsoft.Win32;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class ImportSchedulesWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();

        public ImportSchedulesWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
            {
                PathTextBox.Text = dialog.FileName;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                ErrorTextBlock.Text = "Выберите файл.";
                return;
            }

            var result = _repository.ImportSchedules(PathTextBox.Text);
            new ImportResultWindow(result) { Owner = this }.ShowDialog();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
