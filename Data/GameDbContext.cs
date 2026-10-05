using GameBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PlayerResources> PlayerResources { get; set; }
    public DbSet<Scene> Scenes { get; set; }
    public DbSet<SceneProgress> SceneProgresses { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<PlayerItem> PlayerItems { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<PlayerAchievement> PlayerAchievements { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User - PlayerResources (1 : 1)
        modelBuilder.Entity<PlayerResources>()
            .HasOne(p => p.User)
            .WithOne(u => u.Resources)
            .HasForeignKey<PlayerResources>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - SceneProgress (1 : many)
        modelBuilder.Entity<SceneProgress>()
            .HasOne(p => p.User)
            .WithMany(u => u.SceneProgress)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - SceneProgress (1 : many)
        modelBuilder.Entity<SceneProgress>()
            .HasOne(p => p.Scene)
            .WithMany(s => s.SceneProgress)
            .HasForeignKey(p => p.SceneId)
            .OnDelete(DeleteBehavior.Cascade);

        // One progress record per user per scene
        modelBuilder.Entity<SceneProgress>()
            .HasIndex(p => new { p.UserId, p.SceneId })
            .IsUnique();

        // Health must be between 0 and 5
        modelBuilder.Entity<PlayerResources>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_PlayerResources_Health",
                "\"Health\" >= 0 AND \"Health\" <= 5"
            ));
        
        // User - PlayerItem (1 : many)
        modelBuilder.Entity<PlayerItem>()
            .HasOne(p => p.User)
            .WithMany(u => u.PlayerItems)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item - PlayerItem (1 : many)
        modelBuilder.Entity<PlayerItem>()
            .HasOne(p => p.Item)
            .WithMany(i => i.PlayerItems)
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // One item can be owned only once by user
        modelBuilder.Entity<PlayerItem>()
            .HasIndex(p => new { p.UserId, p.ItemId })
            .IsUnique();

        // User - PlayerAchievement (1 : many)
        modelBuilder.Entity<PlayerAchievement>()
            .HasOne(p => p.User)
            .WithMany(u => u.Achievements)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Achievement - PlayerAchievement (1 : many)
        modelBuilder.Entity<PlayerAchievement>()
            .HasOne(p => p.Achievement)
            .WithMany(a => a.PlayerAchievements)
            .HasForeignKey(p => p.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // One achievement can be unlocked only once by a user
        modelBuilder.Entity<PlayerAchievement>()
            .HasIndex(p => new { p.UserId, p.AchievementId })
            .IsUnique();

        // User - EmailVerificationToken (1 : many)
        modelBuilder.Entity<EmailVerificationToken>()
            .HasOne(t => t.User)
            .WithMany(u => u.EmailVerificationTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - PasswordResetToken (1 : many)
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(t => t.User)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }   
}
// public DbSet<PlayerStatistics> PlayerStatistics { get; set; }