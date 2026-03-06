using System.Windows;

namespace Amonic.App.Views
{
    public partial class ImportSchedulesWindow : Window
    {
        public ImportSchedulesWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет выбор CSV-файла.");
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            new ImportResultWindow { Owner = this }.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
