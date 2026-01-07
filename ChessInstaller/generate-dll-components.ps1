#!/usr/bin/env pwsh
# Generate WiX component definitions for all DLL files

param(
    [string]$SourceDir = ".\ChessInstaller\Release",
    [string]$OutputFile = ".\ChessInstaller\RuntimeDlls.wix"
)

Write-Host "Generating DLL components from: $SourceDir"

# Get all DLL files except the main app DLLs
$dlls = Get-ChildItem "$SourceDir\*.dll" | Where-Object { 
    $_.Name -notlike "Chess.dll" -and 
    $_.Name -notlike "SplashKitSDK.dll" -and 
    $_.Name -notlike "Newtonsoft.Json.dll"
}

Write-Host "Found $($dlls.Count) runtime DLLs to include"

# Start building the WiX XML
$xml = @"
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
  <Fragment>
    <!-- Auto-generated .NET Runtime DLLs -->
    <ComponentGroup Id="RuntimeDllComponents" Directory="INSTALLFOLDER">
"@

# Add each DLL as a file in the same component
$xml += @"

      <Component Id="RuntimeDlls" Guid="B1234567-1234-1234-1234-123456789030">
"@

$counter = 1
foreach ($dll in $dlls) {
    $xml += "`n        <File Id=`"RuntimeDll$counter`" Source=`"`$(var.BinDir)\$($dll.Name)`" />"
    $counter++
}

$xml += @"

      </Component>
    </ComponentGroup>
  </Fragment>
</Wix>
"@

# Write to file
$xml | Out-File -FilePath $OutputFile -Encoding UTF8
Write-Host "✓ Generated $OutputFile with $($counter - 1) DLL files"
