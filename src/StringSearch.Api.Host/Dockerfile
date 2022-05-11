FROM mcr.microsoft.com/dotnet/aspnet:6.0.5-alpine3.14
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]