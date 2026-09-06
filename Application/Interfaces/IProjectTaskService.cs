using Application.DTOs;
using Domain.Common;

namespace Application.Common.Interfaces;

public interface IProjectTaskService
{
    Task<ProjectTaskDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProjectTaskDto>> GetByProjectIdAsync(Guid projectId);
    Task<Result<ProjectTaskDto>> CreateAsync(CreateProjectTaskRequest request);
    Task<Result<ProjectTaskDto>> UpdateAsync(Guid id, UpdateProjectTaskRequest request);
    Task<bool> DeleteAsync(Guid id);
}
