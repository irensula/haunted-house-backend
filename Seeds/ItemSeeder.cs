using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class ItemSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        if (await context.Items.AnyAsync()) return; 
        // User seed
        var item = new Item { 
            Name = "Book",
            Description = "Has to be in the bookshelves",
        };

        context.Items.Add(item);
        
        await context.SaveChangesAsync();
    }
}