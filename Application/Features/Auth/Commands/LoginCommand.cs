using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands;

public sealed record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponse>>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(command.Request.Email);
        if (user == null ||
            !_passwordHasher.Verify(command.Request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Fail("Email or password is incorrect.");
        }

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
