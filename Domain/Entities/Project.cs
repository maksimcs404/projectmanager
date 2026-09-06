using repeating.Domain.Common;

namespace repeating.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    private Project(string name, string description, Guid ownerId)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Project> Create(string name, string description, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Project>.Fail("Project name can not be empty.");

        if (ownerId == Guid.Empty)
            return Result<Project>.Fail("Project owner is required.");

        return Result<Project>.Success(new Project(name, description, ownerId));
    }
}
