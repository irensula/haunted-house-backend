namespace GameBackend.Models;

public class Scene
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;

    // Mavigation property
    public ICollection<SceneProgress> SceneProgress { get; set; } = new List<SceneProgress>();

}