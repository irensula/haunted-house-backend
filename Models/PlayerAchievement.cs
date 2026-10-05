namespace GameBackend.Models;

public class PlayerAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}