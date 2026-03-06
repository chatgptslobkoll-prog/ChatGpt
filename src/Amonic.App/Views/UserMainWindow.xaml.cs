using System.Windows;
using Amonic.App.Services;

namespace Amonic.App.Views
{
    public partial class UserMainWindow : Window
    {
        private readonly int _userId;
        private readonly int _activityLogId;
        private readonly AppRepository _repository = new AppRepository();

        public UserMainWindow(int userId, int activityLogId)
        {
            InitializeComponent();
            _userId = userId;
            _activityLogId = activityLogId;
            LoadData();
        }

        private void LoadData()
        {
            var data = _repository.GetUserDashboard(_userId);
            WelcomeTextBlock.Text = $"Hi {data.fullName}, Welcome to AMONIC Airlines Automation System";
            TimeTextBlock.Text = $"Time spent on system: {data.timeSpent}";
            CrashTextBlock.Text = $"Number of crashes: {data.crashes}";
            ActivityDataGrid.ItemsSource = data.logs;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _repository.CloseActivityLog(_activityLogId);
        }
    }
}
