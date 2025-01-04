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
        return appData.Users;
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

    // Utility: Ensure the data folder exists
    private void EnsureFolderExists()
    {
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}




