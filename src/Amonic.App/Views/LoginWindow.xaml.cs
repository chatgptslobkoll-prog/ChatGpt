using System;
using System.Windows;
using System.Windows.Threading;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new AuthService();
        private readonly DispatcherTimer _lockTimer;
        private int _failedAttempts;
        private int _lockSecondsLeft;

        public LoginWindow()
        {
            InitializeComponent();

            _lockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _lockTimer.Tick += LockTimerOnTick;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_lockSecondsLeft > 0)
            {
                MessageTextBlock.Text = $"Повторная попытка через {_lockSecondsLeft} сек.";
                return;
            }

            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageTextBlock.Text = "Заполните email и пароль.";
                return;
            }

            var result = _authService.Login(email, password);
            if (!result.Success)
            {
                MessageTextBlock.Text = result.ErrorMessage;

                if (!result.IsInfrastructureError)
                {
                    _failedAttempts++;
                    if (_failedAttempts >= 3)
                    {
                        StartLockCountdown(10);
                    }
                }

                return;
            }

            _failedAttempts = 0;

            Window targetWindow = result.RoleId == 1
                ? (Window)new AdminMainWindow()
                : new UserMainWindow();

            targetWindow.Owner = this;
            Hide();
            targetWindow.ShowDialog();
            Show();
        }

        private void StartLockCountdown(int seconds)
        {
            _lockSecondsLeft = seconds;
            LoginButton.IsEnabled = false;
            _lockTimer.Start();
            MessageTextBlock.Text = $"Слишком много попыток. Подождите {_lockSecondsLeft} сек.";
        }

        private void LockTimerOnTick(object sender, EventArgs e)
        {
            _lockSecondsLeft--;
            if (_lockSecondsLeft <= 0)
            {
                _lockTimer.Stop();
                _failedAttempts = 0;
                LoginButton.IsEnabled = true;
                MessageTextBlock.Text = "Можно попробовать снова.";
                return;
            }

            MessageTextBlock.Text = $"Слишком много попыток. Подождите {_lockSecondsLeft} сек.";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
