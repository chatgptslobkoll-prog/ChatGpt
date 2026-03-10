using System.Windows;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class ImportResultWindow : Window
    {
        public ImportResultWindow(ImportResultDto result)
        {
            InitializeComponent();
            AddedTextBlock.Text = $"Добавлено: {result.Added}";
            OtherTextBlock.Text = $"Обновлено: {result.Updated}, Дубликаты: {result.Duplicates}, Ошибки: {result.Invalid}";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
