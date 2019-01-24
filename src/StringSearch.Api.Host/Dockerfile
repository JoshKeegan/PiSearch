FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY out ./
ENTRYPOINT ["dotnet", "StringSearch.Api.Host.dll"]