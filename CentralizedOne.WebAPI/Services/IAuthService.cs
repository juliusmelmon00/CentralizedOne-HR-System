using CentralizedOne.Data;
using CentralizedOne.Data.Models;


namespace CentralizedOne.WebAPI.Services
{
    public interface IAuthService
    {
        string Authenticate(User user);
        User? ValidateUser(string username, string password, ApplicationDbContext db);
    }
}
