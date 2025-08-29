# Test simple de la configuration PostgreSQL
Write-Host "Test de configuration PostgreSQL - FleetSyncManager" -ForegroundColor Green
Write-Host "===================================================" -ForegroundColor Green

Write-Host "`n1. Verification de la configuration dans appsettings.json..." -ForegroundColor Yellow
$configPath = "src\Laroche.FleetManager.Web\appsettings.json"

if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    $useInMemory = $config.DatabaseSettings.UseInMemoryDatabase
    $connectionString = $config.ConnectionStrings.DefaultConnection
    
    Write-Host "   UseInMemoryDatabase: $useInMemory" -ForegroundColor White
    Write-Host "   Connection String: $(($connectionString -split ';')[0..3] -join ';')..." -ForegroundColor White
    
    if ($useInMemory -eq $false) {
        Write-Host "   Configuration PostgreSQL: ACTIVE" -ForegroundColor Green
    } else {
        Write-Host "   Configuration InMemory: ACTIVE (Incorrect!)" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "   Fichier appsettings.json non trouve!" -ForegroundColor Red
    exit 1
}

Write-Host "`n2. Test du build..." -ForegroundColor Yellow
dotnet build --configuration Debug --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Build: REUSSI" -ForegroundColor Green
} else {
    Write-Host "   Build: ECHEC" -ForegroundColor Red
    exit 1
}

Write-Host "`n3. Verification des migrations EF..." -ForegroundColor Yellow
$migrationsPath = "src\Laroche.FleetManager.Infrastructure\Migrations"

if (Test-Path $migrationsPath) {
    $migrations = Get-ChildItem $migrationsPath -Filter "*.cs" | Where-Object { $_.Name -ne "ApplicationDbContextModelSnapshot.cs" }
    Write-Host "   Migrations existantes: $($migrations.Count)" -ForegroundColor Green
    
    foreach ($migration in $migrations) {
        Write-Host "     - $($migration.Name)" -ForegroundColor Cyan
    }
} else {
    Write-Host "   Aucune migration trouvee - il faudra creer une migration initiale" -ForegroundColor Yellow
}

Write-Host "`n4. RESUME:" -ForegroundColor Magenta
Write-Host "   Configuration PostgreSQL: ACTIVE" -ForegroundColor Green
Write-Host "   Build: REUSSI" -ForegroundColor Green

if (Test-Path $migrationsPath) {
    Write-Host "   Migrations EF: PRETES" -ForegroundColor Green
} else {
    Write-Host "   Migrations EF: A CREER" -ForegroundColor Yellow
}

Write-Host "`nLes parametres ont ete modifies avec succes!" -ForegroundColor Green
Write-Host "L'application utilisera PostgreSQL au lieu d'InMemory." -ForegroundColor Green

Write-Host "`nPour demarrer l'application:" -ForegroundColor White
Write-Host "dotnet run --project src/Laroche.FleetManager.Web" -ForegroundColor Cyan

Write-Host "`nNote:" -ForegroundColor Yellow
Write-Host "Assurez-vous que PostgreSQL est demarre avant de lancer l'application." -ForegroundColor Yellow
Write-Host "Commande Docker pour PostgreSQL:" -ForegroundColor Yellow
Write-Host "docker run --name postgres-fleet -e POSTGRES_PASSWORD=FleetSync123! -p 5432:5432 -d postgres:15" -ForegroundColor White
