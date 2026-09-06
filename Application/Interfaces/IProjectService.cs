using Application.DTOs;
using Domain.Common;

namespace Application.Common.Interfaces;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProjectDto>> GetByOwnerIdAsync(Guid ownerId);
    Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request);
    Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(Guid id);
}
