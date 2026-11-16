using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class ItemSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        // items seed        
        var items = new List<Item> { 
            new Item
            {
                SceneId = 1,
                Name = "Book",
                Description = "Has to be in the bookshelves",
            },
            new Item
            {
                SceneId = 1,
                Name = "Flashlight",
                Description = "Lights the room",
            },
            new Item
            {
                SceneId = 1,
                Name = "Map",
                Description = "Shows the locality",
            },
            new Item
            {
                SceneId = 1,
                Name = "Bag",
                Description = "Contains food",
            },
            new Item
            {
                SceneId = 1,
                Name = "Key",
                Description = "Opens the door",
            }
        };

        foreach (var item in items)
        {
            var exists = await context.Items.AnyAsync(
                i => i.SceneId == item.SceneId &&
                    i.Name == item.Name
            );

            if (!exists)
            {
                context.Items.Add(item);
            }
        }
        
        await context.SaveChangesAsync();
    }
}