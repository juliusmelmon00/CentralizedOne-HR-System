using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CentralizedOne.EmployeeApp.Views.Shared
{
    public partial class ToastNotification : Border
    {
        public ToastNotification(string message, string icon, string colorHex)
        {
            InitializeComponent();
            DataContext = new
            {
                Message = message,
                Icon = icon,
                BackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorHex))
            };

            Loaded += async (s, e) => await AnimateAsync();
        }

        private async Task AnimateAsync()
        {
            // Start hidden (slide down and transparent)
            this.Opacity = 0;
            this.RenderTransform = new TranslateTransform(0, 40); // 40px slide from bottom

            // ✅ Fade & Slide In animation
            var storyboard = new Storyboard();

            // Opacity animation
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(fadeIn, this);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeIn);

            // Slide in animation
            var slideIn = new DoubleAnimation(40, 0, TimeSpan.FromMilliseconds(300));
            Storyboard.SetTarget(slideIn, this);
            Storyboard.SetTargetProperty(slideIn, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(slideIn);

            storyboard.Begin();

            // ✅ Stay visible for a moment
            await Task.Delay(2500);

            // ✅ Fade Out + Slide Down On Exit
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(250));
            var slideOut = new DoubleAnimation(0, 40, TimeSpan.FromMilliseconds(250));

            fadeOut.Completed += (s, e) => (Parent as Panel)?.Children.Remove(this);

            BeginAnimation(OpacityProperty, fadeOut);
            BeginAnimation(RenderTransformProperty, slideOut);
        }

    }
}
