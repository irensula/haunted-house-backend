using System.ComponentModel.DataAnnotations;

namespace GameBackend.DTOs.Auth;

public class ResendVerificationRequest
{
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;
}