namespace GameBackend.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool EmailConfirmed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public PlayerResources? Resources { get; set; }

    public ICollection<SceneProgress> SceneProgress { get; set; } = new List<SceneProgress>();

    public ICollection<PlayerItem> PlayerItems { get; set; } = new List<PlayerItem>();

    public ICollection<PlayerAchievement> Achievements { get; set; } = new List<PlayerAchievement>();

    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();

    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
}