using GameBackend.Data;
using GameBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly GameDbContext _context;

    public PlayerController(GameDbContext context)
    {
        _context = context;
    }

    // get user
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer(int id)
    {
        var player = await _context.Users
            .Include(u => u.Resources)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (player == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            player.Id,
            player.Username,
            player.Email,
            player.EmailConfirmed,
            Resources = player.Resources == null
                ? null
                : new
                {
                    player.Resources.Coins,
                    player.Resources.Diamonds,
                    player.Resources.Health,
                    player.Resources.UpdatedAt
                } 
        });
    }

    // update player's resources
    [HttpPut("{id}/resources")]
    public async Task<IActionResult> UpdateResources(
        int id,
        UpdatePlayerResourcesRequest request)
    {
        var player = await _context.Users
            .Include(u => u.Resources)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (player == null)
        {
            return NotFound();
        }

        if (player.Resources == null)
        {
            return BadRequest("Player resources do not exist.");
        }

        if (request.Health < 0 || request.Health > 5)
        {
            return BadRequest("Health must be between 0 and 5");
        }
        player.Resources.Coins = request.Coins;
        player.Resources.Diamonds = request.Diamonds;
        player.Resources.Health = request.Health;
        player.Resources.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            player.Resources.Coins,
            player.Resources.Diamonds,
            player.Resources.Health,
            player.Resources.UpdatedAt
        });
    }


}