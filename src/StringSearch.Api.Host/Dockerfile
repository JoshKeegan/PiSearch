FROM mcr.microsoft.com/dotnet/aspnet:9.0.2-alpine3.20
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]