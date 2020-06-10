FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.5
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]