if (Test-Path "C:\Repository\MinimalTransform\Deployment\MinimalTransform") {
    Remove-Item -Path "C:\Repository\MinimalTransform\Deployment\MinimalTransform" -Recurse -Force
}

dotnet publish C:\Repository\MinimalTransform\Source\MinimalTransform -c Release -o C:\Repository\MinimalTransform\Deployment\MinimalTransform

# Step 6: Clean up unnecessary development files and .publish.ps1 files
$filesToRemove = @(
    "*.pdb",
    "*.xml",
    "appsettings.Development.json",
    "*.publish.ps1"
)
foreach ($pattern in $filesToRemove) {
    Get-ChildItem -Path "C:\Repository\MinimalTransform\Deployment\MinimalTransform" -Filter $pattern -Recurse | Remove-Item -Force
}

Write-Host "✅ Deployment complete. The application has been published to C:\Repository\MinimalTransform\Deployment\MinimalTransform."