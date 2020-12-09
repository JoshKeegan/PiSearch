FROM mcr.microsoft.com/dotnet/aspnet:5.0.1-alpine3.12
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]