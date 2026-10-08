using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class SceneSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        if (await context.Scenes.AnyAsync()) return; 
        // User seed
        var scene = new Scene { 
            Number = 1,
            Name = "Garden",
        };

        context.Scenes.Add(scene);
        
        await context.SaveChangesAsync();
    }
}