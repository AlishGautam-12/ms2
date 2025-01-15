using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ms2.Models; // Adjust namespace as per your project structure

public class UserService
{
    // Paths for storing application data
    private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
    private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");
    private static User? _loggedInUser;

    // Load AppData (Users, Transactions, Debts) from JSON file
    public AppData LoadData()
    {
        if (!File.Exists(FilePath))
        {
            // If the file doesn't exist, return a new AppData object
            return new AppData();
        }

        try
        {
            // Read JSON content from the file
            var json = File.ReadAllText(FilePath);
            // Deserialize JSON into an AppData object
            return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
        }
        catch (JsonException)
        {
            // Handle corrupted JSON files gracefully
            return new AppData();
        }
        catch (Exception)
        {
            // Handle other potential errors (e.g., file access issues)
            return new AppData();
        }
    }

    // Save AppData (Users, Transactions, Debts) to JSON file
    public void SaveData(AppData data)
    {
        EnsureFolderExists();

        // Serialize AppData object into JSON
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        // Write JSON content to the file
        File.WriteAllText(FilePath, json);
    }

    // Manage Users within AppData
    public List<User> LoadUsers()
    {
        // Load AppData and return the Users list
        var appData = LoadData();
        return appData.Users ?? new List<User>();  // Ensure Users list is not null
    }

    public void SaveUsers(List<User> users)
    {
        // Load the current AppData
        var appData = LoadData();
        // Update the Users list
        appData.Users = users;
        // Save the updated AppData back to the file
        SaveData(appData);
    }

    // Hash a password securely
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    // Validate a password against a stored hash
    public bool ValidatePassword(string inputPassword, string storedPassword)
    {
        var hashedInputPassword = HashPassword(inputPassword);
        return hashedInputPassword == storedPassword;
    }


    public void Logout()
    {
        // If using a session or local state for logged-in user, clear it.
        _loggedInUser = null;

        // Optional: Remove any authentication token or session data (e.g., cookies, local storage)
        // For example, clearing session storage (in Blazor, this could be done via JS interop):
        // sessionStorage.removeItem("user"); // If using JavaScript interop
    }

    // Example of setting the logged-in user (you would use this after successful login)
    public void SetLoggedInUser(User user)
    {
        _loggedInUser = user;
    }

    // Check if a user is logged in (this could return a user object or a boolean flag)
    public bool IsUserLoggedIn()
    {
        return _loggedInUser != null;
    }

    // Calculate Main Balance based on Transactions and Debts
    public decimal CalculateMainBalance()
    {
        var appData = LoadData(); // Load data to access Transactions and Debts

        // Ensure that Transactions and Debts are not null before calculating
        var totalCredit = appData.Transactions?.Sum(t => t.Credit) ?? 0;
        var totalDebit = appData.Transactions?.Sum(t => t.Debit) ?? 0;

        var totalDebtAddedToBalance = appData.Debts?.Sum(d => d.Amount) ?? 0;
        var totalDebtCleared = appData.Debts?.Sum(d => d.PaidAmount) ?? 0;

        // Main balance includes debt added but deducts cleared debt
        return totalCredit - totalDebit + totalDebtAddedToBalance - totalDebtCleared;
    }


    // Update Main Balance in AppData
    public void UpdateMainBalance()
    {
        var appData = LoadData(); // Load data
        appData.MainBalance = CalculateMainBalance(); // Update the main balance

        SaveData(appData); // Save the updated AppData with the new balance
    }

    // Get Main Balance from AppData
    public decimal GetMainBalance()
    {
        var appData = LoadData(); // Load data
        return appData.MainBalance; // Return the current MainBalance
    }

    public bool UserExists(string username, string email)
    {
        var users = LoadUsers();
        return users.Any(u => u.Username == username || u.Email == email);
    }
    public void RegisterUser(string username, string email, string password)
    {
        // Check if the username or email already exists
        if (UserExists(username, email))
        {
            throw new InvalidOperationException("Username or Email already exists.");
        }

        // Create a new user and hash the password
        var newUser = new User
        {
            Username = username,
            Email = email,
            Password = HashPassword(password) // Hash the password securely
        };

        // Load the current list of users
        var users = LoadUsers();

        // Add the new user to the list
        users.Add(newUser);

        // Save the updated list of users
        SaveUsers(users);
    }


    // Utility: Ensure the data folder exists
    private void EnsureFolderExists()
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}

