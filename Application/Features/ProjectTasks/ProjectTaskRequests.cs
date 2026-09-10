using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.ProjectTasks;

public record GetProjectTaskByIdQuery(Guid Id) : IRequest<ProjectTaskDto?>;

public class GetProjectTaskByIdQueryHandler
    : IRequestHandler<GetProjectTaskByIdQuery, ProjectTaskDto?>
{
    private readonly IProjectTaskService _taskService;

    public GetProjectTaskByIdQueryHandler(IProjectTaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<ProjectTaskDto?> Handle(
        GetProjectTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        return _taskService.GetByIdAsync(request.Id);
    }
}

public record GetProjectTasksQuery(Guid ProjectId)
    : IRequest<IReadOnlyList<ProjectTaskDto>>;

public class GetProjectTasksQueryHandler
    : IRequestHandler<GetProjectTasksQuery, IReadOnlyList<ProjectTaskDto>>
{
    private readonly IProjectTaskService _taskService;

    public GetProjectTasksQueryHandler(IProjectTaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<IReadOnlyList<ProjectTaskDto>> Handle(
        GetProjectTasksQuery request,
        CancellationToken cancellationToken)
    {
        return _taskService.GetByProjectIdAsync(request.ProjectId);
    }
}

public record CreateProjectTaskCommand(CreateProjectTaskRequest Request)
    : IRequest<Result<ProjectTaskDto>>;

public class CreateProjectTaskCommandHandler
    : IRequestHandler<CreateProjectTaskCommand, Result<ProjectTaskDto>>
{
    private readonly IProjectTaskService _taskService;

    public CreateProjectTaskCommandHandler(IProjectTaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<Result<ProjectTaskDto>> Handle(
        CreateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        return _taskService.CreateAsync(request.Request);
    }
}

public record UpdateProjectTaskCommand(
    Guid Id,
    UpdateProjectTaskRequest Request) : IRequest<Result<ProjectTaskDto>>;

public class UpdateProjectTaskCommandHandler
    : IRequestHandler<UpdateProjectTaskCommand, Result<ProjectTaskDto>>
{
    private readonly IProjectTaskService _taskService;

    public UpdateProjectTaskCommandHandler(IProjectTaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<Result<ProjectTaskDto>> Handle(
        UpdateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        return _taskService.UpdateAsync(request.Id, request.Request);
    }
}

public record DeleteProjectTaskCommand(Guid Id) : IRequest<bool>;

public class DeleteProjectTaskCommandHandler
    : IRequestHandler<DeleteProjectTaskCommand, bool>
{
    private readonly IProjectTaskService _taskService;

    public DeleteProjectTaskCommandHandler(IProjectTaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<bool> Handle(
        DeleteProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        return _taskService.DeleteAsync(request.Id);
    }
}
