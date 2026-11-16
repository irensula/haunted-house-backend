$ProjectPath = Split-Path -Parent $PSScriptRoot

dotnet restore "$ProjectPath/GameBackend.csproj"