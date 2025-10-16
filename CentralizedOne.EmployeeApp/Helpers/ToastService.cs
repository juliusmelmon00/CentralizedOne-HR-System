using System.Windows;
using System.Windows.Controls;
using CentralizedOne.EmployeeApp.Views.Shared;

namespace CentralizedOne.EmployeeApp.Helpers
{
    public static class ToastService
    {
        private static Panel? _hostPanel;

        public static void Initialize(Panel hostPanel)
        {
            _hostPanel = hostPanel;
        }

        public static void ShowSuccess(string message)
        {
            Show(message, "✅", "#22C55E"); // Green
        }

        public static void ShowError(string message)
        {
            Show(message, "⚠️", "#EF4444"); // Red
        }

        public static void ShowWarning(string message)
        {
            Show(message, "⚠", "#EAB308"); // Yellow
        }

        private static void Show(string message, string icon, string color)
        {
            if (_hostPanel == null) return;

            var toast = new ToastNotification(message, icon, color);
            _hostPanel.Children.Add(toast);
        }
    }
}
