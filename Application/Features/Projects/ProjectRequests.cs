using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Projects;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

public class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectService _projectService;

    public GetProjectByIdQueryHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public Task<ProjectDto?> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        return _projectService.GetByIdAsync(request.Id);
    }
}

public record GetProjectsByOwnerQuery(Guid OwnerId)
    : IRequest<IReadOnlyList<ProjectDto>>;

public class GetProjectsByOwnerQueryHandler
    : IRequestHandler<GetProjectsByOwnerQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectService _projectService;

    public GetProjectsByOwnerQueryHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public Task<IReadOnlyList<ProjectDto>> Handle(
        GetProjectsByOwnerQuery request,
        CancellationToken cancellationToken)
    {
        return _projectService.GetByOwnerIdAsync(request.OwnerId);
    }
}

public record CreateProjectCommand(CreateProjectRequest Request)
    : IRequest<Result<ProjectDto>>;

public class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    private readonly IProjectService _projectService;

    public CreateProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        return _projectService.CreateAsync(request.Request);
    }
}

public record UpdateProjectCommand(Guid Id, UpdateProjectRequest Request)
    : IRequest<Result<ProjectDto>>;

public class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    private readonly IProjectService _projectService;

    public UpdateProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public Task<Result<ProjectDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        return _projectService.UpdateAsync(request.Id, request.Request);
    }
}

public record DeleteProjectCommand(Guid Id) : IRequest<bool>;

public class DeleteProjectCommandHandler
    : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IProjectService _projectService;

    public DeleteProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public Task<bool> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        return _projectService.DeleteAsync(request.Id);
    }
}
