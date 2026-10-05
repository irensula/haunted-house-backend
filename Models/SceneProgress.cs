namespace GameBackend.Models;

public class SceneProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SceneId { get; set; }
    public int CurrentRound { get; set; }
    public bool Completed { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Scene Scene { get; set; } = null!;
}