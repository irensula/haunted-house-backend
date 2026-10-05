namespace GameBackend.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public ICollection<PlayerItem> PlayerItems { get; set; } = new List<PlayerItem>();
}