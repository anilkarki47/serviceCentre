using System.Text.Json;

namespace BikeServiceCentre.Data;

public static class UsersService
{
    public const string SeedUsername = "admin";
    public const string SeedPassword = "admin";
    public static User LoggedIn;

    private static void SaveAll(List<User> users)
    {
        string appDataDirectoryPath = Utils.GetAppDirectoryPath();
        string appUsersFilePath = Utils.GetAppUsersFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(appUsersFilePath, json);

    }

    public static List<User> GetAll()
    {
        string appUsersFilePath = Utils.GetAppUsersFilePath();
        if (!File.Exists(appUsersFilePath))
        {
            return new List<User>();
        }

        var json = File.ReadAllText(appUsersFilePath);

        return JsonSerializer.Deserialize<List<User>>(json);
    }

    public static void SaveUserJson(List<User> user)
    {
        string json = JsonSerializer.Serialize(user);
        System.IO.File.WriteAllText("C:\\Users\\ASUS\\Downloads\\maui\\BikeServiceCentre\\User\\userData.json", json);
    }
    public static List<User> GetUsers()
    {
        string path = "C:\\Users\\ASUS\\Downloads\\maui\\BikeServiceCentre\\User\\userData.json";
        if (!System.IO.File.Exists(path))
        {
            System.IO.File.Create(path);
        }
        string json = System.IO.File.ReadAllText(path);
        List<User> user = JsonSerializer.Deserialize<List<User>>(json);
        return user;
    }

    public static List<User> Create(Guid userId, string username, string password, Role role)
    {
        List<User> users = GetAll();
        List<User> jsonUsers = GetUsers();
        bool usernameExists = users.Any(x => x.Username == username);

        if (usernameExists)
        {
            throw new Exception("Username already exists.");
        }
        List<User> admins = users.FindAll(user => user.Role == Role.Admin);

        if (role == Role.Admin && admins.Count > 1)
        {
            throw new Exception("Only two admins is allowed.");
        }
        else
        {
            users.Add(new User
            {
                Username = username,
                PasswordHash = Utils.HashSecret(password),
                Role = role,
                CreatedBy = userId
            });
            jsonUsers.Add(new User
            {
                Username = username,
                PasswordHash = Utils.HashSecret(password),
                Role = role,
                CreatedBy = userId
            });
        }
        SaveAll(users);
        SaveUserJson(jsonUsers);
        return users;
    }

    public static void SeedUsers()
    {
        var users = GetAll().FirstOrDefault(x => x.Role == Role.Admin);

        if (users == null)
        {
            Create(Guid.Empty, SeedUsername, SeedPassword, Role.Admin);
        }
    }

    public static User GetById(Guid id)
    {
        List<User> users = GetAll();
        return users.FirstOrDefault(x => x.Id == id);
    }
    public static void DeleteByUserId(Guid userId)
    {
        string todosFilePath = Utils.GetAppItemsFilePath(userId);
        if (File.Exists(todosFilePath))
        {
            File.Delete(todosFilePath);
        }
    }
    public static List<User> Delete(Guid id)
    {
        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        DeleteByUserId(id);
        users.Remove(user);
        SaveAll(users);

        return users;
    }


    public static User Login(string username, string password)
    {
        var loginErrorMessage = "Invalid username or password.";
        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Username == username);

        if (user == null)
        {
            throw new Exception(loginErrorMessage);
        }

        bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception(loginErrorMessage);
        }

        LoggedIn = user;
        return user;

    }


    public static User ChangePassword(Guid id, string currentPassword, string newPassword)
    {
        if (currentPassword == newPassword)
        {
            throw new Exception("New password must be different from current password.");
        }

        List<User> users = GetAll();
        User user = users.FirstOrDefault(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        bool passwordIsValid = Utils.VerifyHash(currentPassword, user.PasswordHash);

        if (!passwordIsValid)
        {
            throw new Exception("Incorrect current password.");
        }

        user.PasswordHash = Utils.HashSecret(newPassword);
        user.HasInitialPassword = false;
        SaveAll(users);

        return user;
    }
}