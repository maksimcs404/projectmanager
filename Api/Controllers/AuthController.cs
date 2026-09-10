using Application.DTOs;
using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var result = await _sender.Send(new RegisterCommand(request));
        if (!result.IsSuccess || result.Data == null)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _sender.Send(new LoginCommand(request));
        if (!result.IsSuccess || result.Data == null)
            return Unauthorized(result.Message);

        return Ok(result.Data);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await _sender.Send(new RefreshTokenCommand(request));
        if (!result.IsSuccess || result.Data == null)
            return Unauthorized(result.Message);

        return Ok(result.Data);
    }
}
