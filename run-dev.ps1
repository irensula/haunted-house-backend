# Builds and starts the GameBackend API in Development mode.
# Uses the DLL to avoid Windows blocking the generated .exe.
# Runs the server on http://localhost:5237.
# Open Swagger UI at http://localhost:5237/swagger/index.html.

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5237"

dotnet build

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

dotnet bin/Debug/net10.0/GameBackend.dll