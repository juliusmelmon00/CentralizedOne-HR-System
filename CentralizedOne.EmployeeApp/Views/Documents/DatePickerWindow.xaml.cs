using System;
using System.Windows;

namespace CentralizedOne.EmployeeApp.Views.Documents
{
    public partial class DatePickerWindow : Window
    {
        public DateTime SelectedDate { get; private set; }

        public DatePickerWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerControl.SelectedDate.HasValue)
            {
                SelectedDate = DatePickerControl.SelectedDate.Value;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a valid date.");
            }
        }
    }
}
