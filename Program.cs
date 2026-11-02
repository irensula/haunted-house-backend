using GameBackend.Data;
using GameBackend.Seeds;
using Microsoft.EntityFrameworkCore;
using GameBackend.Configuration;
using GameBackend.Services;
using Microsoft.AspNetCore.Identity;
using GameBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine(
    $"DefaultConnection configured: {!string.IsNullOrWhiteSpace(connectionString)}");

builder.Services.AddDbContext<GameDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();

builder.Services
    .AddOptions<ResendOptions>()
    .Bind(builder.Configuration.GetSection(ResendOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.ApiKey) &&
        !string.IsNullOrWhiteSpace(options.FromEmail) &&
        !string.IsNullOrWhiteSpace(options.FromName) &&
        Uri.TryCreate(options.ApiUrl, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps,
        "Resend configuration is invalid.")
    .ValidateOnStart();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddHttpClient<EmailService>();

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

    await DbSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "GameBackend API v1");
    });
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();