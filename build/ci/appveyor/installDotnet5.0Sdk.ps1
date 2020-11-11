# Install .NET SDK 5.0 onto the appveyor build server.
# Workaround until they add it to the official image.
# src (originally for older SDK): https://github.com/appveyor/ci/issues/2746#issuecomment-444292816

Write-Host "Installing .NET 5.0 SDK..." -ForegroundColor Cyan
Write-Host "Downloading..."
$exePath = "$env:TEMP\dotnet-sdk-5.0.100-win-x64.exe"
(New-Object Net.WebClient).DownloadFile('https://download.visualstudio.microsoft.com/download/pr/2892493e-df43-409e-af68-8b14aa75c029/53156c889fc08f01b7ed8d7135badede/dotnet-sdk-5.0.100-win-x64.exe', $exePath)
Write-Host "Installing..."
cmd /c start /wait "$exePath" /quiet /norestart
del $exePath
Write-Host "Installed" -ForegroundColor Green