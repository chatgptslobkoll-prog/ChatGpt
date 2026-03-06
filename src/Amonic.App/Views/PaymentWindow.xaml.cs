using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Amonic.App.Services;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class PaymentWindow : Window
    {
        private readonly List<int> _scheduleIds;
        private readonly int _cabinTypeId;
        private readonly List<PassengerInput> _passengers;
        private readonly AppRepository _repository = new AppRepository();

        public PaymentWindow(List<int> scheduleIds, int cabinTypeId, List<PassengerInput> passengers)
        {
            InitializeComponent();
            _scheduleIds = scheduleIds;
            _cabinTypeId = cabinTypeId;
            _passengers = passengers;
            var firstPrice = scheduleIds.Count > 0 ? _repository.SearchSchedules(null, null, null, string.Empty, "Date and Time").FirstOrDefault(s => s.ID == scheduleIds[0])?.Economy ?? 0 : 0;
            var multiplier = _cabinTypeId == 1 ? 1m : _cabinTypeId == 2 ? 1.35m : 1.35m * 1.30m;
            var total = Math.Floor(firstPrice * multiplier) * scheduleIds.Count * passengers.Count;
            TotalAmountTextBlock.Text = $"Total amount: {total}";
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var issue = _repository.IssueTickets(SessionContext.CurrentUserId, _scheduleIds, _cabinTypeId, _passengers);
                new TicketSummaryWindow(issue) { Owner = this }.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Ошибка выпуска билетов: " + ex.Message;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
