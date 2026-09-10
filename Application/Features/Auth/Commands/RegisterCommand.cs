using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands;

public sealed record RegisterCommand(CreateUserRequest Request) : IRequest<Result<AuthResponse>>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(
        IUserService userService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context,
        IJwtProvider jwtProvider)
    {
        _userService = userService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(command.Request);
        if (!result.IsSuccess || result.Data == null)
            return Result<AuthResponse>.Fail(result.Message ?? "User creation failed.");

        var user = await _userRepository.GetByEmailAsync(command.Request.Email);
        if (user == null)
            return Result<AuthResponse>.Fail("User was not created.");

        var refreshToken = _jwtProvider.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            User = user,
            Expires = DateTime.UtcNow.AddDays(7),
            IsActive = true
        });

        await _context.SaveChangesAsync();

        return Result<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = _jwtProvider.GenerateAccessToken(user),
            RefreshToken = refreshToken
        });
    }
}
