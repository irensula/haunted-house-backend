namespace GameBackend.Models;

public class PlayerItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public DateTime FoundAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public User User { get; set; } = null!;
    public Item Item { get; set; } = null!;
}