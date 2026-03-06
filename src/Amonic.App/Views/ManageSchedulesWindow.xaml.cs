using System.Windows;

namespace Amonic.App.Views
{
    public partial class ManageSchedulesWindow : Window
    {
        public ManageSchedulesWindow()
        {
            InitializeComponent();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            new ImportSchedulesWindow { Owner = this }.ShowDialog();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            new EditScheduleWindow { Owner = this }.ShowDialog();
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет переключение Confirmed/Cancelled для выбранного рейса.");
        }
    }
}
