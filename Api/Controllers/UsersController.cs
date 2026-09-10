using Application.DTOs;
using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _sender.Send(new GetUsersQuery()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!IsAdmin() && GetCurrentUserId() != id)
            return Forbid();

        var user = await _sender.Send(new GetUserByIdQuery(id));
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateUserRequest request)
    {
        if (!IsAdmin() && GetCurrentUserId() != id)
            return Forbid();

        if (!IsAdmin())
        {
            var currentUser = await _sender.Send(new GetUserByIdQuery(id));
            if (currentUser == null)
                return NotFound();

            request.Role = currentUser.Role;
        }

        var result = await _sender.Send(new UpdateUserCommand(id, request));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _sender.Send(new DeleteUserCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
