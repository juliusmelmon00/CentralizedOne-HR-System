using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CentralizedOne.EmployeeApp.Helpers
{
    public sealed class StatusToColorConverter : IValueConverter // ✅ "sealed" helps XAML compiler resolve correctly
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Gray;

            string status = value.ToString()?.ToLower() ?? "";

            return status switch
            {
                "approved" => new SolidColorBrush(Color.FromRgb(34, 197, 94)),   // Green (#22C55E)
                "pending" => new SolidColorBrush(Color.FromRgb(234, 179, 8)),   // Yellow (#EAB308)
                "rejected" => new SolidColorBrush(Color.FromRgb(239, 68, 68)),  // Red (#EF4444)
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing; // ✅ instead of throwing, safer in XAML parsing
        }
    }
}
