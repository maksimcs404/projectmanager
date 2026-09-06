namespace Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool IsActive { get; set; }
    public Guid UserId { get; set; }
    
    public User User { get; set; }
}