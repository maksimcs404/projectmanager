using Application.Common.Interfaces;
using Application.DTOs;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return Result<UserDto>.Fail("User with this email already exists.");

        var result = User.Create(
            request.Name,
            request.Email,
            _passwordHasher.Hash(request.Password),
            "User");

        if (!result.IsSuccess || result.Data == null)
            return Result<UserDto>.Fail(result.Message ?? "User creation failed.");

        await _userRepository.AddAsync(result.Data);
        await _context.SaveChangesAsync();

        return Result<UserDto>.Success(MapToDto(result.Data));
    }

    public async Task<Result<UserDto>> UpdateAsync(
        Guid id,
        UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return Result<UserDto>.Fail("User not found.");

        user.Name = request.Name;
        user.Email = request.Email;
        user.Role = request.Role;

        _userRepository.Update(user);
        await _context.SaveChangesAsync();

        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        _userRepository.Delete(user);
        await _context.SaveChangesAsync();

        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
