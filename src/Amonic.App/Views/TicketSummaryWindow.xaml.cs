using System.Windows;

namespace Amonic.App.Views
{
    public partial class TicketSummaryWindow : Window
    {
        public TicketSummaryWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
