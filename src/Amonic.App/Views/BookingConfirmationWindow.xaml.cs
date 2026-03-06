using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Amonic.App.Services;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class BookingConfirmationWindow : Window
    {
        private readonly List<FlightOptionItem> _selectedFlights;
        private readonly int _passengerCount;
        private readonly int _cabinTypeId;
        private readonly AppRepository _repository = new AppRepository();
        private readonly List<PassengerInput> _passengers = new List<PassengerInput>();

        public BookingConfirmationWindow(List<FlightOptionItem> selectedFlights, int passengerCount, int cabinTypeId)
        {
            InitializeComponent();
            _selectedFlights = selectedFlights;
            _passengerCount = passengerCount;
            _cabinTypeId = cabinTypeId;

            RoutesDataGrid.ItemsSource = _selectedFlights;
            CountryComboBox.ItemsSource = _repository.GetCountries();
            RefreshPassengers();
        }

        private void AddPassenger_Click(object sender, RoutedEventArgs e)
        {
            if (_passengers.Count >= _passengerCount)
            {
                ErrorTextBlock.Text = "Пассажиров уже добавлено достаточно.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) || string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportTextBox.Text) || string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                !BirthdatePicker.SelectedDate.HasValue || !(CountryComboBox.SelectedValue is int countryId))
            {
                ErrorTextBlock.Text = "Заполните обязательные поля пассажира.";
                return;
            }

            _passengers.Add(new PassengerInput
            {
                FirstName = FirstNameTextBox.Text.Trim(),
                LastName = LastNameTextBox.Text.Trim(),
                Birthdate = BirthdatePicker.SelectedDate,
                PassportNumber = PassportTextBox.Text.Trim(),
                PassportCountryID = countryId,
                Phone = PhoneTextBox.Text.Trim(),
                Email = EmailTextBox.Text.Trim()
            });

            ErrorTextBlock.Text = string.Empty;
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            PassportTextBox.Clear();
            PhoneTextBox.Clear();
            EmailTextBox.Clear();
            BirthdatePicker.SelectedDate = null;
            RefreshPassengers();
        }

        private void RefreshPassengers()
        {
            PassengersDataGrid.ItemsSource = null;
            PassengersDataGrid.ItemsSource = _passengers;
            Title = $"Booking Confirmation ({_passengers.Count}/{_passengerCount})";
        }

        private void RemovePassenger_Click(object sender, RoutedEventArgs e)
        {
            var selected = PassengersDataGrid.SelectedItem as PassengerInput;
            if (selected == null) return;
            _passengers.Remove(selected);
            RefreshPassengers();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (_passengers.Count != _passengerCount)
            {
                ErrorTextBlock.Text = $"Нужно добавить {_passengerCount} пассажиров.";
                return;
            }

            var scheduleIds = _selectedFlights.Select(x => x.ScheduleID).ToList();
            var paymentWindow = new PaymentWindow(scheduleIds, _cabinTypeId, _passengers) { Owner = this };
            paymentWindow.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
