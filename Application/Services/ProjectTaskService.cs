using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

public class ProjectTaskService : IProjectTaskService
{
    private readonly IProjectTaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;

    public ProjectTaskService(
        IProjectTaskRepository taskRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _context = context;
    }

    public async Task<ProjectTaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IReadOnlyList<ProjectTaskDto>> GetByProjectIdAsync(Guid projectId)
    {
        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<Result<ProjectTaskDto>> CreateAsync(
        CreateProjectTaskRequest request)
    {
        var result = ProjectTask.Create(
            request.Title,
            request.Description,
            request.ProjectId,
            request.Priority,
            request.AssignedToId,
            request.DueDate);

        if (!result.IsSuccess || result.Data == null)
            return Result<ProjectTaskDto>.Fail(result.Message ?? "Task creation failed.");

        await _taskRepository.AddAsync(result.Data);
        await _context.SaveChangesAsync();

        return Result<ProjectTaskDto>.Success(MapToDto(result.Data));
    }

    public async Task<Result<ProjectTaskDto>> UpdateAsync(
        Guid id,
        UpdateProjectTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return Result<ProjectTaskDto>.Fail("Task not found.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
        task.AssignedToId = request.AssignedToId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        _taskRepository.Update(task);
        await _context.SaveChangesAsync();

        return Result<ProjectTaskDto>.Success(MapToDto(task));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            return false;

        _taskRepository.Delete(task);
        await _context.SaveChangesAsync();

        return true;
    }

    private static ProjectTaskDto MapToDto(ProjectTask task)
    {
        return new ProjectTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            AssignedToId = task.AssignedToId,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
