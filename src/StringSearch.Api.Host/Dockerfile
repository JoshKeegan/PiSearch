FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-alpine3.18
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]