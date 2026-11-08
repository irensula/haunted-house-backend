using System.Security.Claims;
using GameBackend.Data;
using GameBackend.DTOs.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameBackend.Models;

namespace GameBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameDbContext _context;

    public GameController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetGame()
    {
        return Ok(new
        {
            name = "Haunted House",
            version = "1.0.0",
            status = "online"
        });
    }

    // update scene
    [HttpPost("round-completed")]
    public async Task<IActionResult> CompleteRound(UpdateSceneProgressRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        // check player
        var player = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == parsedUserId);

        if (player == null)
        {
            return NotFound("Player not found");
        }

        // check scene
        var sceneExists = await _context.Scenes
            .AnyAsync(s => s.Id == request.SceneId);

        if (!sceneExists)
        {
            return NotFound("Scene not found.");
        }

        const int totalRounds = 3;

        if (request.CompletedRound < 1 || request.CompletedRound > totalRounds)
        {
            return BadRequest("Invalid round number.");
        }

        // find player's progress in the scene
        var progress = await _context.SceneProgresses
            .FirstOrDefaultAsync(s => 
            s.UserId == player.Id &&
            s.SceneId == request.SceneId);

        if (progress == null)
        {
            if (request.CompletedRound != 1)
            {
                return BadRequest("You must complete the previous round first.");
            }

            progress = new SceneProgress
            {
                UserId = player.Id,
                SceneId = request.SceneId,
                CurrentRound = 1,
                Completed = false,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SceneProgresses.Add(progress);
        } else
        {
            if (request.CompletedRound != progress.CurrentRound + 1)
            {
                return BadRequest("Invalid round sequence.");
            }

            progress.CurrentRound = request.CompletedRound;
            progress.Completed = progress.CurrentRound == totalRounds;
            progress.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Round completed successfully.",
            progress.SceneId,
            progress.CurrentRound,
            progress.Completed,
            progress.UpdatedAt
        });
    }
}