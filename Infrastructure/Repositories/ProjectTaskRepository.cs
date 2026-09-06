using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectTaskRepository : IProjectTaskRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectTaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectTask?> GetByIdAsync(Guid id)
    {
        return await _context.ProjectTasks
            .FirstOrDefaultAsync(task => task.Id == id);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectTasks
            .Where(task => task.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task AddAsync(ProjectTask task)
    {
        await _context.ProjectTasks.AddAsync(task);
    }

    public void Update(ProjectTask task)
    {
        _context.ProjectTasks.Update(task);
    }

    public void Delete(ProjectTask task)
    {
        _context.ProjectTasks.Remove(task);
    }
}
