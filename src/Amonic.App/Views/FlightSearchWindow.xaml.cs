using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Amonic.App.Services;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class FlightSearchWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();

        public FlightSearchWindow()
        {
            InitializeComponent();
            LoadAirports();
            OutboundDatePicker.SelectedDate = DateTime.Today;
            ReturnDatePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void LoadAirports()
        {
            var airports = _repository.GetAirports();
            FromComboBox.ItemsSource = airports;
            ToComboBox.ItemsSource = new List<AirportItem>(airports);
        }

        private int SelectedCabinTypeId => Convert.ToInt32((CabinComboBox.SelectedItem as ComboBoxItem)?.Tag ?? 1);

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (!(FromComboBox.SelectedValue is int fromId) || !(ToComboBox.SelectedValue is int toId) || !OutboundDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Заполните направление и дату вылета.");
                return;
            }
            if (fromId == toId)
            {
                MessageBox.Show("Аэропорт вылета и прилета не должны совпадать.");
                return;
            }

            OutboundDataGrid.ItemsSource = _repository.SearchFlightOptions(fromId, toId, OutboundDatePicker.SelectedDate.Value, OutboundAroundDaysCheckBox.IsChecked == true, SelectedCabinTypeId);

            if (RoundTripRadioButton.IsChecked == true && ReturnDatePicker.SelectedDate.HasValue)
            {
                ReturnDataGrid.ItemsSource = _repository.SearchFlightOptions(toId, fromId, ReturnDatePicker.SelectedDate.Value, ReturnAroundDaysCheckBox.IsChecked == true, SelectedCabinTypeId);
            }
            else
            {
                ReturnDataGrid.ItemsSource = null;
            }
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PassengersTextBox.Text, out var passengers) || passengers <= 0)
            {
                MessageBox.Show("Введите корректное количество пассажиров.");
                return;
            }

            var outbound = OutboundDataGrid.SelectedItem as FlightOptionItem;
            if (outbound == null)
            {
                MessageBox.Show("Выберите рейс туда.");
                return;
            }

            var selected = new List<FlightOptionItem> { outbound };
            if (RoundTripRadioButton.IsChecked == true)
            {
                var ret = ReturnDataGrid.SelectedItem as FlightOptionItem;
                if (ret == null)
                {
                    MessageBox.Show("Выберите обратный рейс.");
                    return;
                }
                selected.Add(ret);
            }

            foreach (var item in selected)
            {
                if (item.FreeSeats < passengers)
                {
                    MessageBox.Show($"Недостаточно мест на рейсе {item.Flights}.");
                    return;
                }
            }

            new BookingConfirmationWindow(selected, passengers, SelectedCabinTypeId) { Owner = this }.ShowDialog();
        }
    }
}
