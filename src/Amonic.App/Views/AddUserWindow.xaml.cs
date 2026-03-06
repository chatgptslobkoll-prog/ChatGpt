using System;
using System.Linq;
using System.Windows;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();

        public AddUserWindow()
        {
            InitializeComponent();
            OfficeComboBox.ItemsSource = _repository.GetOffices();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;
            var firstName = FirstNameTextBox.Text.Trim();
            var lastName = LastNameTextBox.Text.Trim();
            var officeId = OfficeComboBox.SelectedValue as int?;
            var birthdate = BirthdatePicker.SelectedDate;

            if (new[] { email, password, firstName, lastName }.Any(string.IsNullOrWhiteSpace) || !officeId.HasValue || !birthdate.HasValue)
            {
                ErrorTextBlock.Text = "Все поля обязательны.";
                return;
            }

            try
            {
                _repository.AddUser(email, password, firstName, lastName, officeId.Value, birthdate.Value);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Ошибка сохранения: " + ex.Message;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
