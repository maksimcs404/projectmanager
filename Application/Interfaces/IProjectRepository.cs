using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Project>> GetByOwnerIdAsync(Guid ownerId);
    Task AddAsync(Project project);
    void Update(Project project);
    void Delete(Project project);
}
