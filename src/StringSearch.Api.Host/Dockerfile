FROM mcr.microsoft.com/dotnet/aspnet:10.0.3-alpine3.22
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]