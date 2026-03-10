using System;
using System.Windows;
using System.Windows.Controls;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class ChangeRoleWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();
        private readonly int _userId;

        public ChangeRoleWindow(int userId, string currentRole)
        {
            InitializeComponent();
            _userId = userId;
            CurrentRoleTextBlock.Text = currentRole;
            RoleComboBox.SelectedIndex = (currentRole ?? string.Empty).Contains("Admin") || (currentRole ?? string.Empty).Contains("Админ") ? 0 : 1;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            var selected = RoleComboBox.SelectedItem as ComboBoxItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите роль.");
                return;
            }

            var roleId = Convert.ToInt32(selected.Tag);
            _repository.ChangeUserRole(_userId, roleId);
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
