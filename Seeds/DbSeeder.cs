using GameBackend.Data;

namespace GameBackend.Seeds;

public static class DbSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        await SceneSeeder.SeedAsync(context);
        await ItemSeeder.SeedAsync(context);
        await AchievementSeeder.SeedAsync(context);
        await UserSeeder.SeedAsync(context);
    }
}