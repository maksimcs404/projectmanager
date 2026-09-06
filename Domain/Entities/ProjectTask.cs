using Domain.Common;

namespace Domain.Entities;

public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Todo;
    public ProjectTaskPriority Priority { get; set; } = ProjectTaskPriority.Medium;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    private ProjectTask(
        string title,
        string description,
        Guid projectId,
        ProjectTaskPriority priority,
        Guid? assignedToId,
        DateTime? dueDate)
    {
        Title = title;
        Description = description;
        ProjectId = projectId;
        Priority = priority;
        AssignedToId = assignedToId;
        DueDate = dueDate;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<ProjectTask> Create(
        string title,
        string description,
        Guid projectId,
        ProjectTaskPriority priority = ProjectTaskPriority.Medium,
        Guid? assignedToId = null,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<ProjectTask>.Fail("Task title can not be empty.");

        if (projectId == Guid.Empty)
            return Result<ProjectTask>.Fail("Task project is required.");

        return Result<ProjectTask>.Success(
            new ProjectTask(title, description, projectId, priority, assignedToId, dueDate));
    }
}
