docker run --rm `
    --network gamebackend_default `
    --env-file .env `
    -v "${PWD}:/src" `
    -w /src `
    mcr.microsoft.com/dotnet/sdk:10.0 `
    bash -c "dotnet restore GameBackend.csproj && dotnet tool install --tool-path /tmp/tools dotnet-ef --version 10.0.11 && /tmp/tools/dotnet-ef database update"