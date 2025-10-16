using System;
using System.Windows;

namespace CentralizedOne.EmployeeApp.Views.Documents
{
    public partial class ExpiryDateDialog : Window
    {
        public DateTime SelectedExpiryDate { get; private set; }

        public ExpiryDateDialog()
        {
            InitializeComponent();
            DateInput.SelectedDate = DateTime.Now.AddDays(30); // Default +30 days ✅
            DateInput.DisplayDateStart = DateTime.Now; // Prevent selecting past dates ✅
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (DateInput.SelectedDate.HasValue)
            {
                SelectedExpiryDate = DateInput.SelectedDate.Value;
                DialogResult = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
