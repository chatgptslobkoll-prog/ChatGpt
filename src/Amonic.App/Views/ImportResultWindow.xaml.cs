using System.Windows;

namespace Amonic.App.Views
{
    public partial class ImportResultWindow : Window
    {
        public ImportResultWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
