using System.Security.Claims;
using GameBackend.Data;
using GameBackend.DTOs.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameBackend.Models;

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

    // add player item
    [HttpPost("player-items")]
    public async Task<IActionResult> AddPlayerItem(AddPlayerItemRequest request)
    {
        // get UserId from JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }
        // find player
        var player = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == parsedUserId);

        if (player == null)
        {
            return NotFound();
        }
        // find item
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == request.ItemId);

        if (item == null)
        {
            return NotFound("Item not found.");
        }
        // check if player has this item already
        var alreadyHasItem = await _context.PlayerItems 
            .AnyAsync(p => p.UserId == player.Id && p.ItemId == item.Id);

        if (alreadyHasItem)
        {
            return BadRequest("Player already has this item");
        }
        // add item to PlayerItems
        var playerItem = new PlayerItem
        {
            UserId = player.Id,
            ItemId = item.Id,
            FoundAt = DateTime.UtcNow
        };
        
        _context.PlayerItems.Add(playerItem);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Item added successfully.",
            Item = item.Name,
            playerItem.FoundAt
        });
    }
}