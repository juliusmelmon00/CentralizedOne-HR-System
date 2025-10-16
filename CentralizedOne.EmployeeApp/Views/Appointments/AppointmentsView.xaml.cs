using CentralizedOne.EmployeeApp.Services;
using CentralizedOne.EmployeeApp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CentralizedOne.EmployeeApp.Views.Appointments
{
    public partial class AppointmentsView : UserControl
    {
        public AppointmentsView()
        {
            InitializeComponent();
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            try
            {
                LblStatus.Visibility = Visibility.Visible;
                LblStatus.Text = "⏳ Loading appointments...";

                var appointments = await ApiClient.GetMyAppointmentsAsync();


                if (appointments == null || appointments.Count == 0)
                {
                    LblStatus.Text = "⚠ No appointments found.";
                }
                else
                {
                    LblStatus.Visibility = Visibility.Collapsed;
                    ListAppointments.ItemsSource = appointments;
                }
            }
            catch (Exception ex)
            {
                LblStatus.Text = "❌ Failed to load appointments.";
                Console.WriteLine($"[ERROR] LoadAppointments(): {ex.Message}");
            }
        }

        // 👉 Placeholder for future "Schedule" modal
        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("📌 Scheduling popup will be added next.", "Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
