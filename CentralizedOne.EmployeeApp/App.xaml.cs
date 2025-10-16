using System.Windows;

namespace CentralizedOne.EmployeeApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var login = new Views.Auth.LoginWindow();
            login.Show();
        }
    }
}
