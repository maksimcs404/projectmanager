using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;

    public ProjectService(
        IProjectRepository projectRepository,
        IApplicationDbContext context)
    {
        _projectRepository = projectRepository;
        _context = context;
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        return project == null ? null : MapToDto(project);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetByOwnerIdAsync(Guid ownerId)
    {
        var projects = await _projectRepository.GetByOwnerIdAsync(ownerId);
        return projects.Select(MapToDto).ToList();
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request)
    {
        var result = Project.Create(
            request.Name,
            request.Description,
            request.OwnerId);

        if (!result.IsSuccess || result.Data == null)
            return Result<ProjectDto>.Fail(result.Message ?? "Project creation failed.");

        await _projectRepository.AddAsync(result.Data);
        await _context.SaveChangesAsync();

        return Result<ProjectDto>.Success(MapToDto(result.Data));
    }

    public async Task<Result<ProjectDto>> UpdateAsync(
        Guid id,
        UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return Result<ProjectDto>.Fail("Project not found.");

        project.Name = request.Name;
        project.Description = request.Description;

        _projectRepository.Update(project);
        await _context.SaveChangesAsync();

        return Result<ProjectDto>.Success(MapToDto(project));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return false;

        _projectRepository.Delete(project);
        await _context.SaveChangesAsync();

        return true;
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            CreatedAt = project.CreatedAt
        };
    }
}
