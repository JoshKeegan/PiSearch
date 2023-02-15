FROM mcr.microsoft.com/dotnet/aspnet:7.0.3-alpine3.16
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]