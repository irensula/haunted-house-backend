using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class AchievementSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        if (await context.Achievements.AnyAsync()) return; 

        var achievement = new Achievement { 
            Name = "Achievement name",
           Description = "Achievement description"
        };

        context.Achievements.Add(achievement);
        
        await context.SaveChangesAsync();
    }
}
