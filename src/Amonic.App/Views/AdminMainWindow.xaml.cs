using System.Windows;

namespace Amonic.App.Views
{
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            new AddUserWindow { Owner = this }.ShowDialog();
        }

        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            new ChangeRoleWindow { Owner = this }.ShowDialog();
        }

        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет блокировка/разблокировка выбранного пользователя в реальном времени.");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
