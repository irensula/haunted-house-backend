namespace GameBackend.Models;

public class PlayerResources
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Coins { get; set; }
    public int Diamonds { get; set; }
    public int Health { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public User User { get; set; } = null!;
}