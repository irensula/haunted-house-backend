using System.Security.Claims;
using GameBackend.Data;
using GameBackend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GameBackend.Controllers;

[Authorize]
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
    [HttpGet("profile")]
    public async Task<IActionResult> GetPlayer()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var player = await _context.Users
            .Include(u => u.Resources)
            .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

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
    [HttpPut("resources")]
    public async Task<IActionResult> UpdateResources(UpdatePlayerResourcesRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

         var player = await _context.Users
            .Include(u => u.Resources)
            .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

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