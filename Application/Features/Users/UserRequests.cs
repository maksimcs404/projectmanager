using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Users;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserService _userService;

    public GetUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<IReadOnlyList<UserDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        return _userService.GetAllAsync();
    }
}

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<UserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        return _userService.GetByIdAsync(request.Id);
    }
}

public record UpdateUserCommand(Guid Id, UpdateUserRequest Request)
    : IRequest<Result<UserDto>>;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<Result<UserDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        return _userService.UpdateAsync(request.Id, request.Request);
    }
}

public record DeleteUserCommand(Guid Id) : IRequest<bool>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserService _userService;

    public DeleteUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<bool> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        return _userService.DeleteAsync(request.Id);
    }
}
