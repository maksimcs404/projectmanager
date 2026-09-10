using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result<AuthResponse>>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context,
        IJwtProvider jwtProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(command.Request.RefreshToken);
        if (refreshToken == null ||
            !refreshToken.IsActive ||
            refreshToken.Expires <= DateTime.UtcNow ||
            refreshToken.User == null)
        {
            return Result<AuthResponse>.Fail("Refresh token is invalid.");
        }
        refreshToken.IsActive = false;
        refreshToken.IsActive = false;
        _refreshTokenRepository.Update(refreshToken);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            UserId = refreshToken.User.Id,
            User = refreshToken.User,
            Expires = DateTime.UtcNow.AddDays(7),
            IsActive = true
        });

        await _context.SaveChangesAsync();

        return Result<AuthResponse>.Success(new AuthResponse
        {
            AccessToken = _jwtProvider.GenerateAccessToken(refreshToken.User),
            RefreshToken = newRefreshToken
        });
    }
}
