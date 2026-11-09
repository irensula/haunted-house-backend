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
    [HttpPost("stage-completed")]
    public async Task<IActionResult> CompleteStage(UpdateSceneProgressRequest request)
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

        // check battle stages
        const int totalBattleStages = 3;

        if (request.CompletedStage < 1 || request.CompletedStage > totalBattleStages)
        {
            return BadRequest("Invalid stage number.");
        }

        // find player's progress in the scene
        var progress = await _context.SceneProgresses
            .FirstOrDefaultAsync(s => 
            s.UserId == player.Id &&
            s.SceneId == request.SceneId);

        if (progress == null)
        {
            if (request.CompletedStage != 1)
            {
                return BadRequest("You must complete the previous stage first.");
            }

            progress = new SceneProgress
            {
                UserId = player.Id,
                SceneId = request.SceneId,
                CurrentStage = 1,
                Completed = false,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SceneProgresses.Add(progress);
        } else
        {
            if (request.CompletedStage != progress.CurrentStage + 1)
            {
                return BadRequest("Invalid stage sequence.");
            }

            progress.CurrentStage = request.CompletedStage;
            progress.Completed = false;
            progress.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Stage completed successfully.",
            progress.SceneId,
            progress.CurrentStage,
            progress.Completed,
            progress.UpdatedAt
        });
    }

    // get progress
    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        var progress = await _context.SceneProgresses
            .Where(p => p.UserId == parsedUserId)
            .Select(p =>  new
                {
                    p.Id,
                    p.SceneId,
                    SceneName = p.Scene.Name,
                    SceneNumber = p.Scene.Number,
                    p.CurrentStage,
                    p.Completed,
                    p.DoorOpened,
                    p.UpdatedAt
                })
            .ToListAsync();

        return Ok(new
        {
            Progress = progress
        });
    }

}