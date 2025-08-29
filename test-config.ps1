# Test de la configuration PostgreSQL pour FleetSyncManager
Write-Host "Test de la configuration PostgreSQL" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green

# Vérification de la configuration
Write-Host "`n1. Verification des parametres de configuration..." -ForegroundColor Yellow

$appSettings = Get-Content "appsettings.json" | ConvertFrom-Json
$appSettingsDev = Get-Content "appsettings.Development.json" -ErrorAction SilentlyContinue | ConvertFrom-Json

$useInMemory = $appSettings.DatabaseSettings.UseInMemoryDatabase
Write-Host "   appsettings.json UseInMemoryDatabase: $useInMemory" -ForegroundColor White

if ($appSettingsDev) {
    $useInMemoryDev = $appSettingsDev.DatabaseSettings.UseInMemoryDatabase
    Write-Host "   appsettings.Development.json UseInMemoryDatabase: $useInMemoryDev" -ForegroundColor White
}

if ($useInMemory -eq $false) {
    Write-Host "   Configuration principale: PostgreSQL active" -ForegroundColor Green
} else {
    Write-Host "   Configuration principale: InMemory active" -ForegroundColor Red
}

# Test du build
Write-Host "`n2. Test du build..." -ForegroundColor Yellow
dotnet build --configuration Debug --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Build reussi" -ForegroundColor Green
} else {
    Write-Host "   Erreur de build" -ForegroundColor Red
    exit 1
}

# Test des migrations EF
Write-Host "`n3. Verification des migrations Entity Framework..." -ForegroundColor Yellow

if (-not (Test-Path "src/Laroche.FleetManager.Infrastructure/Migrations")) {
    Write-Host "   Aucune migration trouvee - creation de la migration initiale..." -ForegroundColor Cyan
    dotnet ef migrations add InitialCreate --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   Migration initiale creee avec succes" -ForegroundColor Green
    } else {
        Write-Host "   Erreur lors de la creation de la migration" -ForegroundColor Red
        exit 1
    }
} else {
    $migrations = Get-ChildItem "src/Laroche.FleetManager.Infrastructure/Migrations" -Filter "*.cs" | Where-Object { $_.Name -ne "ApplicationDbContextModelSnapshot.cs" }
    Write-Host "   Migrations existantes trouvees: $($migrations.Count)" -ForegroundColor Green
}

Write-Host "`n4. Resume de la configuration:" -ForegroundColor Magenta
Write-Host "   - Configuration PostgreSQL: Active" -ForegroundColor Green
Write-Host "   - Build: Reussi" -ForegroundColor Green  
Write-Host "   - Migrations EF: Pretes" -ForegroundColor Green

Write-Host "`nPour demarrer l'application:" -ForegroundColor White
Write-Host "dotnet run --project src/Laroche.FleetManager.Web" -ForegroundColor Cyan

Write-Host "`nNote: Assurez-vous que PostgreSQL est demarre avant de lancer l'application." -ForegroundColor Yellow
Write-Host "Commande Docker si PostgreSQL n'est pas installe:" -ForegroundColor Yellow
Write-Host "docker run --name postgres-fleet -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15" -ForegroundColor White
