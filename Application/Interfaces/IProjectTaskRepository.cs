using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProjectTaskRepository
{
    Task<ProjectTask?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(Guid projectId);
    Task AddAsync(ProjectTask task);
    void Update(ProjectTask task);
    void Delete(ProjectTask task);
}
