using System.Windows;

namespace Amonic.App.Views
{
    public partial class CrashReasonWindow : Window
    {
        public string CrashReason { get; private set; }

        public CrashReasonWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var reason = ReasonTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                ErrorTextBlock.Text = "Причина обязательна.";
                return;
            }

            CrashReason = reason;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
