using GameBackend.Data;
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
}