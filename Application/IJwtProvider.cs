namespace Application;

public interface IJwtProvider
{
    public string GenerateAccessToken();
    public string GenerateRefreshToken();

}