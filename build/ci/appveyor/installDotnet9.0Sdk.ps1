# Install .NET SDK 9.0 onto the appveyor build server.
# Workaround until they add it to the official image.
# src (originally for older SDK): https://github.com/appveyor/ci/issues/2746#issuecomment-444292816

Write-Host "Installing .NET 9.0 SDK..." -ForegroundColor Cyan
Write-Host "Downloading..."
$exePath = "$env:TEMP\dotnet-sdk-9.0.100-win-x64.exe"
(New-Object Net.WebClient).DownloadFile('https://download.visualstudio.microsoft.com/download/pr/10bb041d-e705-473e-9654-27c0e038f5bd/447c0c10654c2949872fa6154b8c27b5/dotnet-sdk-9.0.100-win-x64.exe', $exePath)
Write-Host "Installing..."
cmd /c start /wait "$exePath" /quiet /norestart
del $exePath
Write-Host "Installed" -ForegroundColor Green