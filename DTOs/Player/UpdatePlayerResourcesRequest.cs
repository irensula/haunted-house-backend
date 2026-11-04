namespace GameBackend.DTOs.Player;

public class UpdatePlayerResourcesRequest
{
    public int Coins { get; set; }
    public int Diamonds { get; set; }
    public int Health { get; set; }
}