# Script de diagnostic de configuration
Write-Host "=== Diagnostic de Configuration ==="
Write-Host ""

# Vérifier les variables d'environnement
Write-Host "Variables d'environnement:"
Write-Host "ASPNETCORE_ENVIRONMENT: $env:ASPNETCORE_ENVIRONMENT"
Write-Host ""

# Vérifier les fichiers de configuration
Write-Host "Fichiers de configuration:"
Get-ChildItem "src/Laroche.FleetManager.Web/appsettings*.json" | ForEach-Object {
    Write-Host "- $($_.Name)"
}
Write-Host ""

# Afficher le contenu de appsettings.Development.json
Write-Host "=== appsettings.Development.json ==="
Get-Content "src/Laroche.FleetManager.Web/appsettings.Development.json" | Select-Object -First 20
Write-Host ""

# Test avec dotnet user-secrets
Write-Host "=== User Secrets ==="
try {
    $secrets = dotnet user-secrets list --project src/Laroche.FleetManager.Web 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host $secrets
    } else {
        Write-Host "Aucun user secret configuré"
    }
} catch {
    Write-Host "Erreur lors de la lecture des user secrets"
}
Write-Host ""

# Tester la configuration EF Core directement
Write-Host "=== Test EF Core Configuration ==="
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef dbcontext info --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
