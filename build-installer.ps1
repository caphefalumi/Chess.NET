#!/usr/bin/env pwsh

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Chess.NET WiX Installer Builder" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

# Step 1: Build Chess.NET
Write-Host "`n[Step 1] Building Chess.NET in Release mode..." -ForegroundColor Yellow
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Step 2: Publish Chess.NET
Write-Host "`n[Step 2] Publishing Chess.NET..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained false
if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish failed!" -ForegroundColor Red
    exit 1
}

# Step 3: Create Release folder
Write-Host "`n[Step 3] Staging files for installer..." -ForegroundColor Yellow
$ReleasePath = ".\ChessInstaller\Release"
mkdir $ReleasePath -Force | Out-Null

# Determine the build output path (check both net10.0 and net9.0)
$BuildPath = if (Test-Path ".\bin\Release\net10.0") { ".\bin\Release\net10.0" } else { ".\bin\Release\net9.0" }
Write-Host "  Using build path: $BuildPath"

# Copy files
Write-Host "  - Copying Chess.exe..."
copy "$BuildPath\Chess.exe" "$ReleasePath\"

Write-Host "  - Copying Chess.dll..."
copy "$BuildPath\Chess.dll" "$ReleasePath\"

Write-Host "  - Copying SplashKitSDK.dll..."
copy "$BuildPath\SplashKitSDK.dll" "$ReleasePath\"

Write-Host "  - Copying Newtonsoft.Json.dll..."
copy "$BuildPath\Newtonsoft.Json.dll" "$ReleasePath\"

Write-Host "  - Copying dependency files..."
copy "$BuildPath\Chess.deps.json" "$ReleasePath\" -ErrorAction SilentlyContinue
copy "$BuildPath\Chess.runtimeconfig.json" "$ReleasePath\" -ErrorAction SilentlyContinue

Write-Host "  - Copying Resources folder..."
Copy-Item "$BuildPath\Resources" "$ReleasePath\" -Recurse -Force

# Verify files
if (-not (Test-Path "$ReleasePath\Chess.exe")) {
    Write-Host "Chess.exe not found!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ All files staged successfully" -ForegroundColor Green

# Step 4: Build WiX MSI
Write-Host "`n[Step 4] Building WiX MSI installer..." -ForegroundColor Yellow
$OutputPath = ".\ChessInstaller\Output"
mkdir $OutputPath -Force | Out-Null

$BinDir = Resolve-Path "$ReleasePath"
Write-Host "  Using BinDir: $BinDir"

try {
    wix build -out "$OutputPath\Chess.NET.msi" `
      -d BinDir="$BinDir" `
      ".\ChessInstaller\Product.wix" `
      ".\ChessInstaller\Features.wix"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✓ MSI built successfully!" -ForegroundColor Green
    } else {
        Write-Host "  ✗ WiX build failed!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "  ✗ Error running WiX: $_" -ForegroundColor Red
    Write-Host "`n  Tip: Ensure WiX is installed and in PATH" -ForegroundColor Yellow
    exit 1
}

# Step 5: Verify MSI
Write-Host "`n[Step 5] Verifying installer..." -ForegroundColor Yellow
$MsiPath = Resolve-Path "$OutputPath\Chess.NET.msi"
$MsiSize = (Get-Item $MsiPath).Length / 1MB
Write-Host "  MSI file: $MsiPath" -ForegroundColor Cyan
Write-Host "  Size: $([Math]::Round($MsiSize, 2)) MB" -ForegroundColor Cyan
Write-Host "  ✓ Installer ready for testing!" -ForegroundColor Green

Write-Host "`n================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Test installation: msiexec /i '$MsiPath'"
Write-Host "2. Verify shortcuts and functionality"
Write-Host "3. Test uninstallation"
Write-Host "4. Upload to GitHub or OneDrive for public access"
Write-Host "5. Include in your deployment report`n"
