using System.Collections.Generic;
using System.Windows;
using Amonic.App.Services;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class AdminMainWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();

        public AdminMainWindow()
        {
            InitializeComponent();
            LoadOffices();
            LoadUsers();
        }

        private void LoadOffices()
        {
            var offices = new List<OfficeItem> { new OfficeItem { ID = 0, Title = "Все офисы" } };
            offices.AddRange(_repository.GetOffices());
            OfficeComboBox.ItemsSource = offices;
            OfficeComboBox.SelectedIndex = 0;
        }

        private void LoadUsers()
        {
            var selectedOfficeId = OfficeComboBox.SelectedValue as int?;
            if (selectedOfficeId == 0)
            {
                selectedOfficeId = null;
            }

            UsersDataGrid.ItemsSource = _repository.GetUsers(selectedOfficeId);
        }

        private UserListItem GetSelectedUser()
        {
            return UsersDataGrid.SelectedItem as UserListItem;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddUserWindow { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void ManageSchedules_Click(object sender, RoutedEventArgs e)
        {
            new ManageSchedulesWindow { Owner = this }.ShowDialog();
        }

        private void BookTickets_Click(object sender, RoutedEventArgs e)
        {
            new FlightSearchWindow { Owner = this }.ShowDialog();
        }

        private void ChangeRole_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            var dlg = new ChangeRoleWindow(selected.ID, selected.Role) { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedUser();
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }

            _repository.ToggleUserActive(selected.ID);
            LoadUsers();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void OfficeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                LoadUsers();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
