using GameBackend.Data;
using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Seeds;

public static class UserSeeder
{
    public static async Task SeedAsync(GameDbContext context)
    {
        if (await context.Users.AnyAsync()) return; 
        // User seed
        var user = new User { 
            Username = "Jane",
            Email = "janedoe@gmail.com",
            PasswordHash = "TEMP_HASH",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Resources seed
        var resources = new PlayerResources { 
            UserId = user.Id,
            Coins = 30,
            Diamonds = 10,
            Health = 4,
            UpdatedAt = DateTime.UtcNow
        };

        // Progress seed
        var progress = new SceneProgress { 
            UserId = user.Id,
            SceneId = 1,
            CurrentRound = 3,
            Completed = true,
            UpdatedAt = DateTime.UtcNow
        };

        // Items seed
        var items = new PlayerItem
        {
            UserId = user.Id,
            ItemId = 1,
            FoundAt = DateTime.UtcNow
        };

        // Achievement seed
        var achievement = new PlayerAchievement
        {
            UserId = user.Id,
            AchievementId = 1,
            UnlockedAt = DateTime.UtcNow
        };

        // Email token seed
        var emailToken = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = "EmailVerificationToken",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Used = true
        };

        // Password reset token
        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = "PasswordResetToken",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Used = true,
        };

        context.PlayerResources.Add(resources);
        context.SceneProgresses.Add(progress);
        context.PlayerItems.Add(items);
        context.PlayerAchievements.Add(achievement);
        context.EmailVerificationTokens.Add(emailToken);
        context.PasswordResetTokens.Add(passwordResetToken);
        
        await context.SaveChangesAsync();

        }
}