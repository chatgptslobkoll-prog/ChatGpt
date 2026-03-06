using System.Windows;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class ImportResultWindow : Window
    {
        public ImportResultWindow(ImportResultDto result)
        {
            InitializeComponent();
            AddedTextBlock.Text = $"Added: {result.Added}";
            OtherTextBlock.Text = $"Updated: {result.Updated}, Duplicates: {result.Duplicates}, Invalid: {result.Invalid}";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
