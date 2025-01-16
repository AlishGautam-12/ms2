using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ms2.Models;

namespace ms2.Components.Pages
{
    public partial class Register : ComponentBase
    {

       
    private string RegisterUsername = "";
        private string RegisterPassword = "";
        private string RegisterEmail = "";
        private string Message = "";
        private bool IsRegistrationSuccessful = false;

        private async Task RegisterUser()
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword))
            {
                Message = "Username and password are required.";
                return;
            }

            // Load existing users
            var users = UserService.LoadUsers();

            // Check if the username already exists
            if (users.Any(u => u.Username == RegisterUsername))
            {
                // Inform the user that the username is already taken
                Message = "Username already exists. Please login.";
                await Task.Delay(2000);
                NavigationManager.NavigateTo("/login");
                return;
            }

            // Create new user and hash the password
            var newUser = new User
            {
                Username = RegisterUsername,
                Password = UserService.HashPassword(RegisterPassword),
                Email = RegisterEmail
            };

            // Save the new user
            users.Add(newUser);
            UserService.SaveUsers(users);

            // Set successful registration message
            Message = "Registration successful! Redirecting to login...";
            IsRegistrationSuccessful = true;
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/login");
        }
    }
}

