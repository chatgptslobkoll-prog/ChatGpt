using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Amonic.App.Services;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class ManageSchedulesWindow : Window
    {
        private readonly AppRepository _repository = new AppRepository();

        public ManageSchedulesWindow()
        {
            InitializeComponent();
            LoadAirports();
            Search();
        }

        private void LoadAirports()
        {
            var airports = new List<AirportItem> { new AirportItem { ID = 0, IATACode = "ALL" } };
            airports.AddRange(_repository.GetAirports());
            FromComboBox.ItemsSource = airports;
            ToComboBox.ItemsSource = new List<AirportItem>(airports);
            FromComboBox.SelectedIndex = 0;
            ToComboBox.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;
        }

        private void Search()
        {
            var fromId = (FromComboBox.SelectedValue is int f && f != 0) ? (int?)f : null;
            var toId = (ToComboBox.SelectedValue is int t && t != 0) ? (int?)t : null;
            var date = DatePicker.SelectedDate;
            var sort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Date and Time";
            SchedulesDataGrid.ItemsSource = _repository.SearchSchedules(fromId, toId, date, FlightNoTextBox.Text.Trim(), sort);
        }

        private ScheduleListItem SelectedSchedule => SchedulesDataGrid.SelectedItem as ScheduleListItem;

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ImportSchedulesWindow { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                Search();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = SelectedSchedule;
            if (selected == null)
            {
                MessageBox.Show("Выберите рейс.");
                return;
            }

            var dlg = new EditScheduleWindow(selected.ID, selected.Date, selected.Time, selected.Economy) { Owner = this };
            if (dlg.ShowDialog() == true)
            {
                Search();
            }
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            var selected = SelectedSchedule;
            if (selected == null)
            {
                MessageBox.Show("Выберите рейс.");
                return;
            }

            _repository.ToggleSchedule(selected.ID);
            Search();
        }
    }
}
