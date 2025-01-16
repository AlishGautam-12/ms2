using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ms2.Models;

namespace ms2.Components.Pages
{
    public partial class Login : ComponentBase
    {

    
    private string LoginUsername = "";
        private string LoginPassword = "";
        private string Message = "";

        private async Task LoginUser()
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(LoginUsername) || string.IsNullOrWhiteSpace(LoginPassword))
            {
                Message = "Username and password are required.";
                return;
            }

            // Load existing users
            var users = UserService.LoadUsers();

            // Find the user by username
            var user = users.FirstOrDefault(u => u.Username == LoginUsername);

            if (user == null)
            {
                Message = "Invalid username or password.";
                return;
            }

            // Validate the password
            if (!UserService.ValidatePassword(LoginPassword, user.Password))
            {
                Message = "Invalid username or password.";
                return;
            }

            // Successful login
            Message = "Login successful! Redirecting to home...";
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/dashboard");
        }
    }
}

