namespace GameBackend.DTOs;

public class UpdatePlayerResourcesRequest
{
    public int Coins { get; set; }
    public int Diamonds { get; set; }
    public int Health { get; set; }
}