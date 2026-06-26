using MedSync.Models;

namespace MedSync.Services;

public class AuthService
{
    public User? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;

    public bool Login(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            CurrentUser = new User
            {
                Username = username,
                FullName = "مدیر سیستم",
                Role = UserRole.Doctor
            };
            return true;
        }

        return false;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}