using Application.Common.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProjectTasksController : ControllerBase
{
    private readonly IProjectTaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;

    public ProjectTasksController(
        IProjectTaskService taskService,
        IProjectService projectService,
        IUserService userService)
    {
        _taskService = taskService;
        _projectService = projectService;
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task != null && !await CanManageProject(task.ProjectId))
            return Forbid();

        return task == null ? NotFound() : Ok(task);
    }

    [HttpGet("project/{projectId:guid}")]
    public async Task<IActionResult> GetByProjectId(Guid projectId)
    {
        if (!await CanManageProject(projectId))
            return Forbid();

        return Ok(await _taskService.GetByProjectIdAsync(projectId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectTaskRequest request)
    {
        if (!await CanManageProject(request.ProjectId))
            return Forbid();

        if (request.AssignedToId.HasValue &&
            await _userService.GetByIdAsync(request.AssignedToId.Value) == null)
        {
            return BadRequest("Assigned user was not found.");
        }

        var result = await _taskService.CreateAsync(request);

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
        UpdateProjectTaskRequest request)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        if (!await CanManageProject(task.ProjectId))
            return Forbid();

        if (request.AssignedToId.HasValue &&
            await _userService.GetByIdAsync(request.AssignedToId.Value) == null)
        {
            return BadRequest("Assigned user was not found.");
        }

        var result = await _taskService.UpdateAsync(id, request);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        if (!await CanManageProject(task.ProjectId))
            return Forbid();

        var deleted = await _taskService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private async Task<bool> CanManageProject(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        if (project == null)
            return false;

        return User.IsInRole("Admin") ||
               GetCurrentUserId() == project.OwnerId;
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
