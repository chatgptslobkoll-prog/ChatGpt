using System.Windows;

namespace Amonic.App.Views
{
    public partial class ChangeRoleWindow : Window
    {
        public ChangeRoleWindow()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет обновление роли выбранного пользователя.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
