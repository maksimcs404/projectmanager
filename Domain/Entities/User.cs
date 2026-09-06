using repeating.Domain.Common;

namespace repeating.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = new Guid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    // public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
    // public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    private User(string name, string email, string password, string role)
    {
        Name = name;
        Email = email;
        Password = password;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
    // todo: continue validations
    public static Result<User> Create(string name, string email, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 20)
            return Result<User>.Fail("Name can not be less than 3 symbols or bigger than 20 symbols.");
        return Result<User>.Success(new User(name, email, password, role));
    }
    
}