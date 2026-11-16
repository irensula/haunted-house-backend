using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class SceneStageSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    { 
        // User seed
        var stages = new List<SceneStage>
        {
            new SceneStage
            {
                SceneId = 1,
                StageNumber = 1,
                TotalUndead = 15,
                TotalCoins = 10,
                TotalDiamonds = 5
            },
            new SceneStage
            {
                SceneId = 1,
                StageNumber = 2,
                TotalUndead = 35,
                TotalCoins = 15,
                TotalDiamonds = 10
            },
            new SceneStage
            {
                SceneId = 1,
                StageNumber = 3,
                TotalUndead = 50,
                TotalCoins = 20,
                TotalDiamonds = 15
            },
            new SceneStage
            {
                SceneId = 2,
                StageNumber = 1,
                TotalUndead = 20,
                TotalCoins = 10,
                TotalDiamonds = 5
            },
            new SceneStage
            {
                SceneId = 2,
                StageNumber = 2,
                TotalUndead = 35,
                TotalCoins = 15,
                TotalDiamonds = 10
            },
            new SceneStage
            {
                SceneId = 2,
                StageNumber = 3,
                TotalUndead = 45,
                TotalCoins = 20,
                TotalDiamonds = 15
            }
        };

        foreach (var stage in stages)
        {
            var exists = await context.SceneStages.AnyAsync(
                s => s.SceneId == stage.SceneId && 
                    s.StageNumber == stage.StageNumber);

            if (!exists)
            {
                context.SceneStages.Add(stage);
            }
        }
        await context.SaveChangesAsync();
    }
}