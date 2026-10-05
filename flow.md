# ASP.NET Core + PostgreSQL + Railway — Backend Setup Workflow

This is the workflow for creating the GameBackend for the Unity game.

## 1. Check .NET version
dotnet --version

The project currently uses .NET 10.

## 2. Create ASP.NET Core Web API project
dotnet new webapi -n GameBackend
cd GameBackend
dotnet run

Expected result:
Now listening on: http://localhost:5237

Stop the application with:
Ctrl + C

## 3. Initial project structure

After creating the project:

GameBackend/
├── Properties/
│   └── launchSettings.json
├── appsettings.json
├── appsettings.Development.json
├── GameBackend.csproj
├── GameBackend.http
└── Program.cs

Later, the structure will look approximately like this:

GameBackend/
├── Data/
│   └── GameDbContext.cs
├── Models/
│   ├── User.cs
│   ├── Scene.cs
│   ├── SceneProgress.cs
│   ├── PlayerResources.cs
│   └── ...
├── Migrations/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── GameBackend.http
└── GameBackend.csproj

## 4. Install dependencies

### Install Entity Framework Core:

dotnet add package Microsoft.EntityFrameworkCore

### Install PostgreSQL provider:

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

### Install EF Core design-time tools:

dotnet add package Microsoft.EntityFrameworkCore.Design

### Check installed packages:

dotnet list package

Expected packages include:

Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Design
Npgsql.EntityFrameworkCore.PostgreSQL

## 5. Create the Models folder

Create:

Models/

Models describe the application's data and are used by Entity Framework Core to create database tables.

For example:

Models/
└── User.cs

Example:

namespace GameBackend.Models;
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

Basic concept:

C# Model
    ↓
Entity Framework Core
    ↓
PostgreSQL Table

## 6. Create the Data folder

Create:

Data/
└── GameDbContext.cs

GameDbContext is the main EF Core class responsible for communication between the application and PostgreSQL.

Example:

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
}

## 7. Configure User Secrets

User Secrets are used for local development secrets such as database passwords.

## Initialize User Secrets:

dotnet user-secrets init

Do not put database passwords directly into:

appsettings.json

or commit them to Git.

## 8. Configure the database connection

For local development, the project connects to Railway PostgreSQL through a Railway tunnel.

The connection string will look like:

Host=127.0.0.1;
Port=51792;
Database=railway;
Username=postgres;
Password=YOUR_PASSWORD

### Store it in User Secrets:

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=51792;Database=railway;Username=postgres;Password=YOUR_PASSWORD"

### Check the secret:

dotnet user-secrets list

## 9. Register GameDbContext in Program.cs

Add:

using GameBackend.Data;
using Microsoft.EntityFrameworkCore;

Get the connection string:

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

Register the database context:

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

The basic architecture is:

ASP.NET Core
      ↓
GameDbContext
      ↓
Entity Framework Core
      ↓
Npgsql
      ↓
PostgreSQL

## 10. Install EF Core CLI tools

### Install dotnet-ef globally:

dotnet tool install --global dotnet-ef

### Check the version:

dotnet ef --version

## 11. Create the first migration

### After creating the models and configuring GameDbContext:

dotnet ef migrations add InitialCreate

EF Core creates:

Migrations/
├── 2026..._InitialCreate.cs
├── 2026..._InitialCreate.Designer.cs
└── GameDbContextModelSnapshot.cs

A migration is an instruction describing how the database schema should change.

This is similar to Knex migrations.

## 12. Install Railway CLI

### On Windows:

npm.cmd install -g @railway/cli

### Check the version:

railway.cmd --version

### Log in:

railway.cmd login

## 13. Link the local project to Railway

Go to the backend project:

cd C:\..\GameBackend

### Link it to the Railway project:

railway.cmd link

Select the appropriate Railway project and environment.

### 14. Create a Railway PostgreSQL tunnel

Run:

railway.cmd connect Postgres-Cd4E --tunnel-only

Railway will provide something similar to:

Host:     127.0.0.1
Port:     51792
User:     postgres
Password:
Database: railway

Keep this terminal window open while using the tunnel.

The connection works like this:

GameBackend
     │
     │ localhost:51792
     ▼
Railway Tunnel
     │
     ▼
Postgres-Cd4E

## 15. Configure the Railway tunnel connection

### Store the tunnel connection in User Secrets:

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=127.0.0.1;Port=51792;Database=railway;Username=postgres;Password=YOUR_PASSWORD"

### Verify:

dotnet user-secrets list

## 16. Apply the migration to Railway PostgreSQL

Run:

dotnet ef database update

EF Core will:

Migrations
    ↓
Entity Framework Core
    ↓
Railway Tunnel
    ↓
Postgres-Cd4E

The database will now contain the tables defined by the migration.

EF Core also creates:

__EFMigrationsHistory

This table keeps track of which migrations have already been applied.

## 17. Add the remaining game models

For the game backend, the planned models are:

Models/
├── User.cs
├── EmailVerificationToken.cs
├── PasswordResetToken.cs
├── Scene.cs
├── SceneProgress.cs
├── PlayerResources.cs
├── Item.cs
├── PlayerItem.cs
├── Achievement.cs
├── PlayerAchievement.cs
└── PlayerStatistics.cs

The main relationships are:

User
 │
 ├── SceneProgress
 │
 ├── PlayerResources
 │
 ├── PlayerItems
 │
 ├── PlayerAchievements
 │
 └── PlayerStatistics

## 18. Create additional migrations

### Whenever the database models change:

dotnet ef migrations add AddGameModels

### Then apply the migration:

dotnet ef database update

Do not delete and recreate the whole database every time.

EF Core tracks applied migrations using:

__EFMigrationsHistory

This is similar to the migration history used by Knex.

## 19. Seed static game data

After the database structure is ready, add initial game data such as:

Scenes
Items
Achievements

For example:

Scenes:
1. Haunted House
2. Basement
3. Garden

Static game data can be seeded.

Player data should not be reset by a seed.

Do not seed/reset:

Users
PlayerResources
SceneProgress
PlayerItems
PlayerAchievements
PlayerStatistics

These contain real player data.

## 20. Create the HTTP API

After the database is ready, start building the API.

Authentication
POST /api/auth/register
POST /api/auth/verify-email
POST /api/auth/login
POST /api/auth/forgot-password
POST /api/auth/reset-password
Game save
GET  /api/game/save
POST /api/game/progress
POST /api/game/round-complete
Player information
GET /api/player
GET /api/player/statistics
GET /api/player/achievements
GET /api/player/items
Leaderboard
GET /api/leaderboard

## 21. Test the API before connecting Unity

Use:

GameBackend.http

For example:

GET http://localhost:5237/api/game

Test the API independently from Unity.

A typical flow:

Register
   ↓
Verify email
   ↓
Login
   ↓
Get game save
   ↓
Update progress
   ↓
Get game save again
   ↓
Verify that the data was saved

This makes debugging much easier.

## 22. Connect Unity to the API

Only after the backend API works independently, connect the Unity game.

The final architecture will be:

                 UNITY GAME
                     │
                    HTTPS
                     │
                     ▼
              ASP.NET Core API
                     │
                     ▼
               Entity Framework
                     │
                     ▼
              PostgreSQL Railway

Unity does not communicate directly with PostgreSQL.

Unity only communicates with the ASP.NET API.

## 23. Deploy the backend to Railway

After the backend works locally:

Local development
       ↓
HTTP testing
       ↓
Unity integration
       ↓
Railway deployment

The production architecture will be:

Unity WebGL
     │
     │ HTTPS
     ▼
Railway ASP.NET Backend
     │
     ▼
Railway PostgreSQL

## Complete Workflow

1.  Check .NET
        ↓
2.  Create ASP.NET Web API
        ↓
3.  Install EF Core + Npgsql + Design
        ↓
4.  Create Models
        ↓
5.  Create GameDbContext
        ↓
6.  Configure User Secrets
        ↓
7.  Register DbContext in Program.cs
        ↓
8.  Install dotnet-ef
        ↓
9.  Create EF migration
        ↓
10. Install Railway CLI
        ↓
11. railway link
        ↓
12. Start Railway PostgreSQL tunnel
        ↓
13. Configure tunnel connection
        ↓
14. dotnet ef database update
        ↓
15. Add remaining models
        ↓
16. Create additional migrations
        ↓
17. Seed static game data
        ↓
18. Build ASP.NET API
        ↓
19. Test API with HTTP requests
        ↓
20. Connect Unity to API
        ↓
21. Deploy backend to Railway


dotnet build
dotnet ef dbcontext list
dotnet ef migrations add AddPlayerResourcesAndScenes