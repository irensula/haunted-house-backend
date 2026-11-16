namespace GameBackend.Models;

public class SceneStage {
    public int Id { get; set; }
    public int SceneId { get; set; }
    public int StageNumber { get; set; }

    public int TotalUndead { get; set; }
    public int TotalCoins { get; set; }
    public int TotalDiamonds { get; set; }

    public Scene Scene { get; set; } = null!;
}