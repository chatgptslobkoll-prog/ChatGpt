using System;
using System.Globalization;
using System.Windows;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class EditScheduleWindow : Window
    {
        private readonly int _scheduleId;
        private readonly AppRepository _repository = new AppRepository();

        public EditScheduleWindow(int scheduleId, DateTime date, TimeSpan time, decimal economy)
        {
            InitializeComponent();
            _scheduleId = scheduleId;
            DatePicker.SelectedDate = date;
            TimeTextBox.Text = time.ToString("hh\\:mm");
            PriceTextBox.Text = economy.ToString(CultureInfo.InvariantCulture);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!DatePicker.SelectedDate.HasValue || !TimeSpan.TryParse(TimeTextBox.Text, out var time) || !decimal.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                ErrorTextBlock.Text = "Проверьте формат даты/времени/цены.";
                return;
            }

            _repository.UpdateSchedule(_scheduleId, DatePicker.SelectedDate.Value, time, price);
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
