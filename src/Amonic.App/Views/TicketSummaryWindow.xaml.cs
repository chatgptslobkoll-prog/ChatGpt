using System.Windows;
using Amonic.App.ViewModels;

namespace Amonic.App.Views
{
    public partial class TicketSummaryWindow : Window
    {
        public TicketSummaryWindow(IssueResult issueResult)
        {
            InitializeComponent();
            BookingReferenceTextBlock.Text = "Код бронирования: " + issueResult.BookingReference;
            TicketsDataGrid.ItemsSource = issueResult.Tickets;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
