using System.Windows;

namespace Amonic.App.Views
{
    public partial class BookingConfirmationWindow : Window
    {
        public BookingConfirmationWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            new PaymentWindow { Owner = this }.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
