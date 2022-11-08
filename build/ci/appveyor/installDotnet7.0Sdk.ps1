# Install .NET SDK 7.0 onto the appveyor build server.
# Workaround until they add it to the official image.
# src (originally for older SDK): https://github.com/appveyor/ci/issues/2746#issuecomment-444292816

Write-Host "Installing .NET 7.0 SDK..." -ForegroundColor Cyan
Write-Host "Downloading..."
$exePath = "$env:TEMP\dotnet-sdk-7.0.100-win-x64.exe"
(New-Object Net.WebClient).DownloadFile('https://download.visualstudio.microsoft.com/download/pr/5b9d1f0d-9c56-4bef-b950-c1b439489b27/b4aa387715207faa618a99e9b2dd4e35/dotnet-sdk-7.0.100-win-x64.exe', $exePath)
Write-Host "Installing..."
cmd /c start /wait "$exePath" /quiet /norestart
del $exePath
Write-Host "Installed" -ForegroundColor Green