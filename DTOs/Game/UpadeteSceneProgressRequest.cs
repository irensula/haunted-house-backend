namespace GameBackend.DTOs.Game;

public class UpdateSceneProgressRequest
{
    public int SceneId { get; set; }
    public int CompletedRound { get; set; }
}