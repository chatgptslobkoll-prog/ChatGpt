using System.Windows;

namespace Amonic.App.Views
{
    public partial class FlightSearchWindow : Window
    {
        public FlightSearchWindow()
        {
            InitializeComponent();
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            new BookingConfirmationWindow { Owner = this }.ShowDialog();
        }
    }
}
