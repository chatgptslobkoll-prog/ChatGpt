using System.Windows;

namespace Amonic.App.Views
{
    public partial class EditScheduleWindow : Window
    {
        public EditScheduleWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Изменения расписания будут сохранены в БД.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
