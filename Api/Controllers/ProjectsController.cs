using Application.DTOs;
using Application.Features.Projects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await _sender.Send(new GetProjectByIdQuery(id));
        if (project != null && !CanManage(project.OwnerId))
            return Forbid();

        return project == null ? NotFound() : Ok(project);
    }

    [HttpGet("owner/{ownerId:guid}")]
    public async Task<IActionResult> GetByOwnerId(Guid ownerId)
    {
        if (!CanManage(ownerId))
            return Forbid();

        return Ok(await _sender.Send(new GetProjectsByOwnerQuery(ownerId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        if (!IsAdmin())
            request.OwnerId = GetCurrentUserId() ?? Guid.Empty;

        var result = await _sender.Send(new CreateProjectCommand(request));
        if (!result.IsSuccess || result.Data == null)
            return BadRequest(result.Message);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            result.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProjectRequest request)
    {
        var project = await _sender.Send(new GetProjectByIdQuery(id));
        if (project == null)
            return NotFound();

        if (!CanManage(project.OwnerId))
            return Forbid();

        var result = await _sender.Send(new UpdateProjectCommand(id, request));
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _sender.Send(new GetProjectByIdQuery(id));
        if (project == null)
            return NotFound();

        if (!CanManage(project.OwnerId))
            return Forbid();

        var deleted = await _sender.Send(new DeleteProjectCommand(id));
        return deleted ? NoContent() : NotFound();
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    private bool CanManage(Guid ownerId)
    {
        return IsAdmin() || GetCurrentUserId() == ownerId;
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
