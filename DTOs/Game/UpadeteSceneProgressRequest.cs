namespace GameBackend.DTOs.Game;

public class UpdateSceneProgressRequest
{
    public int SceneId { get; set; }
    public int CompletedStage { get; set; }
    public int UndeadDefeated { get; set; }
    public int CoinsCollected { get; set; }
    public int DiamondsCollected { get; set; }
}