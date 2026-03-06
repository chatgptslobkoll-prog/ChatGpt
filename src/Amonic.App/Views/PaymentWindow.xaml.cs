using System.Windows;

namespace Amonic.App.Views
{
    public partial class PaymentWindow : Window
    {
        public PaymentWindow()
        {
            InitializeComponent();
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            new TicketSummaryWindow { Owner = this }.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
