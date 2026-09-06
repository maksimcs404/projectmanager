using Application;
using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(
        IUserService userService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var result = await _userService.CreateAsync(request);
        if (!result.IsSuccess || result.Data == null)
            return BadRequest(result.Message);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("User was not created.");

        return Ok(await CreateAuthResponse(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null ||
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Email or password is incorrect.");

        return Ok(await CreateAuthResponse(user));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(
            request.RefreshToken);

        if (refreshToken == null ||
            !refreshToken.IsActive ||
            refreshToken.Expires <= DateTime.UtcNow ||
            refreshToken.User == null)
        {
            return Unauthorized("Refresh token is invalid.");
        }

        refreshToken.IsActive = false;
        _refreshTokenRepository.Update(refreshToken);

        return Ok(await CreateAuthResponse(refreshToken.User));
    }

    private async Task<AuthResponse> CreateAuthResponse(User user)
    {
        var accessToken = _jwtProvider.GenerateAccessToken(user);
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

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}