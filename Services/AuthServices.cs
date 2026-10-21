using System.Security.Cryptography;
using System.Text;
using GameBackend.Configuration;
using GameBackend.Data;
using GameBackend.DTOs.Auth;
using GameBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameBackend.Services;

public class AuthService
{
    private readonly GameDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly EmailService _emailService;
    private readonly ResendOptions _options;

    public AuthService(
        GameDbContext context,
        IPasswordHasher<User> passwordHasher,
        EmailService emailService,
        IOptions<ResendOptions> options)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _options = options.Value;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        // check if username already exists
        var usernameExists = await _context.Users
            .AnyAsync( u => u.Username.ToLower() == username.ToLower(),
            cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException("Username is already taken");
        }

        // check if email already exists
        var emailExists = await _context.Users
            .AnyAsync( u => u.Email.ToLower() == email,
            cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        // create new user
        var user = new User
        {
            Username = username,
            Email = email,
            EmailConfirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var verificationToken = new EmailVerificationToken
        {
            User = user,
            Token = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Used = false
        };

        _context.Users.Add(user);
        _context.EmailVerificationTokens.Add(verificationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var verificationLink = $"{_options.VerificationUrl}?token={Uri.EscapeDataString(rawToken)}";

        var html = $"""
            <h1>Welcome to Haunted House!</h1>
            <p>Please verify your email address by opening the link below on your device with the game installed.</p>
            <p><a href="{System.Net.WebUtility.HtmlEncode(verificationLink)}">Verify email</a></p>
            <p>This link expires in 24 hours.</p>
        """;        

        await _emailService.SendEmailAsync(
            user.Email,
            "Verify your Haunted House account",
            html,
            cancellationToken);

            return new AuthResponse
            {
                Message = "Registration successfull. Please check your email to verify your account"
            };
    }

    public async Task<bool> VerifyEmailAsync(
        string rawToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            return false;
        }

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var verificationToken = await _context.EmailVerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(
                t => t.Token == tokenHash,
                cancellationToken);

        if (verificationToken is null || verificationToken.Used || verificationToken.ExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        verificationToken.User.EmailConfirmed = true;
        verificationToken.Used = true;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<AuthResponse> ResendVerificationAsync(
        string email, CancellationToken cancellationToken= default)
    {
        email = email.Trim().ToLowerInvariant();

        var response = new AuthResponse
        {
            Message = "If an unconfirmed account exists for this email, a verigication email will be sent"
        };

        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Email.ToLower() == email,
            cancellationToken);

        // return the same response if the account doesn't exist or the email has already been confirmed
        if (user is null || user.EmailConfirmed)
        {
            return response;
        }

        // Invalidate previous verification tokens
        var previousTokens = await _context.EmailVerificationTokens
            .Where(t => t.UserId == user.Id && !t.Used)
            .ToListAsync(cancellationToken);

        foreach (var previousToken in previousTokens)
        {
            previousToken.Used = true;
        }

        // generate a new secure token
        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Used = false
        };

        _context.EmailVerificationTokens.Add(verificationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var verificationLink = $"{_options.VerificationUrl}?token={Uri.EscapeDataString(rawToken)}";

        var safeLink = System.Net.WebUtility.HtmlEncode(verificationLink);

        var html = $"""
            <h1>Verify your Haunted House account</h1>
            <p>Please click the link below to verify your email address</p>
            <p><a href="{safeLink}">Verify email</a></p>
            <p>This link is expires in 24 hours.</p>
        """;

        await _emailService.SendEmailAsync(
            user.Email,
            "Verify your Haunted House account",
            html,
            cancellationToken);

        return response;
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        // find user by username or email
        var login = request.UsernameOrEmail.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(
            u => u.Username.ToLower() == login.ToLower() 
                || u.Email.ToLower() == login.ToLower(),
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid username/email or password");
        }

        // verify password
        var result = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed){
            throw new UnauthorizedAccessException("Invalid username/enmil or password");
        }

        // require email verification before login
        if (!user.EmailConfirmed)
        {
            throw new InvalidOperationException("Please verify your email before logging in");
        }

        // update last login time
        user.LastLoginAt = DateTime.UtcNow;

        // save changes including a possible upgraded password hash
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Message = "Login successful"
        };
    }
}
