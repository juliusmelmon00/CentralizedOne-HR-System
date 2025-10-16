using CentralizedOne.WPF.Services;
using System.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializedComponent(); 

        // 🔹 TEMPORARY TEST - Check if DB connection works
        try
        {
            using (var db = DbService.Create())
            {
                int totalUsers = db.Users.Count(); // <-- Runs a query
                MessageBox.Show("✅ Connected! Total Users in DB: " + totalUsers);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("❌ Failed to connect: " + ex.Message);
        }
    }
}
