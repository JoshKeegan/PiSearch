# Install .NET SDK 6.0 onto the appveyor build server.
# Workaround until they add it to the official image.
# src (originally for older SDK): https://github.com/appveyor/ci/issues/2746#issuecomment-444292816

Write-Host "Installing .NET 6.0 SDK..." -ForegroundColor Cyan
Write-Host "Downloading..."
$exePath = "$env:TEMP\dotnet-sdk-6.0.100-win-x64.exe"
(New-Object Net.WebClient).DownloadFile('https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-6.0.100-windows-x64-installer', $exePath)
Write-Host "Installing..."
cmd /c start /wait "$exePath" /quiet /norestart
del $exePath
Write-Host "Installed" -ForegroundColor Green