using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class SceneSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    { 
        // User seed
        var scenes = new List<Scene> { 
            new Scene
            {
                Number = 1,
                Name = "Garden"
            },
            new Scene
            {
                Number = 2,
                Name = "Second Garden"
            }
        };

        foreach (var scene in scenes)
        {
            var exists = await context.Scenes.AnyAsync(
                s => s.Number == scene.Number
            );

            if (!exists)
            {
                context.Scenes.Add(scene);
            }
        }
        
        await context.SaveChangesAsync();
    }
}