using CentralizedOne.EmployeeApp.Helpers;
using CentralizedOne.EmployeeApp.Services;
using CentralizedOne.EmployeeApp.Views.Appointments;
using CentralizedOne.EmployeeApp.Views.Dashboard;
using CentralizedOne.EmployeeApp.Views.Documents;
using CentralizedOne.EmployeeApp.Views.Notifications;
using CentralizedOne.EmployeeApp.Views.Profile;
using CentralizedOne.EmployeeApp.Views.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CentralizedOne.EmployeeApp
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarExpanded = true;

        public MainWindow()
        {
            InitializeComponent();
            ToastService.Initialize(ToastHost);
            LblUser.Text = $"Welcome, {SessionManager.Username}!";

            LoadDefaultPage();
        }

        private void LoadDefaultPage()
        {
            ContentFrame.Content = new DashboardView();
        }

        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = _isSidebarExpanded
                ? (Storyboard)FindResource("CollapseSidebar")
                : (Storyboard)FindResource("ExpandSidebar");

            sb.Begin();
            _isSidebarExpanded = !_isSidebarExpanded;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new DashboardView();
        }

        private void BtnDocuments_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new DocumentsView();
        }

        private void BtnAppointments_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new AppointmentsView();
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new NotificationsView();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new ProfileView();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new SettingsView();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging out...");
            // TODO: Implement logout behavior later
        }
    }
}
