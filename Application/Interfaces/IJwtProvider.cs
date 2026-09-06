using Domain.Entities;

namespace Application;

public interface IJwtProvider
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken();

}