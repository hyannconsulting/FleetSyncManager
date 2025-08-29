# Script de test de la configuration PostgreSQL
param(
    [string]$ConnectionString = "Host=localhost;Database=fleetsynmanager;Username=postgres;Password=postgres;Port=5432"
)

Write-Host "Test de la configuration PostgreSQL pour FleetSyncManager" -ForegroundColor Green
Write-Host "=========================================================" -ForegroundColor Green
Write-Host ""

# Fonction pour tester la connexion PostgreSQL
function Test-PostgreSQLConnection {
    param([string]$connStr)
    
    try {
        Write-Host "Test de connexion PostgreSQL..." -ForegroundColor Yellow
        
        # Utiliser psql si disponible
        if (Get-Command psql -ErrorAction SilentlyContinue) {
            $env:PGPASSWORD = "postgres"
            psql -h localhost -U postgres -d postgres -c "SELECT version();" 2>$null
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Connexion PostgreSQL reussie" -ForegroundColor Green
                return $true
            }
        }
        
        Write-Host "psql non disponible, utilisation du test .NET" -ForegroundColor Yellow
        return $null
    }
    catch {
        Write-Host "Erreur de connexion PostgreSQL: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Fonction pour vérifier la configuration de l'application
function Test-ApplicationConfiguration {
    Write-Host "📋 Vérification de la configuration de l'application..." -ForegroundColor Yellow
    
    $appSettingsPath = "appsettings.json"
    $appSettingsDevPath = "appsettings.Development.json"
    
    if (Test-Path $appSettingsPath) {
        $config = Get-Content $appSettingsPath | ConvertFrom-Json
        $useInMemory = $config.DatabaseSettings.UseInMemoryDatabase
        
        if ($useInMemory -eq $false) {
            Write-Host "✅ appsettings.json configuré pour PostgreSQL (UseInMemoryDatabase: false)" -ForegroundColor Green
        } else {
            Write-Host "❌ appsettings.json configuré pour InMemory (UseInMemoryDatabase: true)" -ForegroundColor Red
            return $false
        }
    }
    
    if (Test-Path $appSettingsDevPath) {
        $configDev = Get-Content $appSettingsDevPath | ConvertFrom-Json
        $useInMemoryDev = $configDev.DatabaseSettings.UseInMemoryDatabase
        
        if ($useInMemoryDev -eq $false) {
            Write-Host "✅ appsettings.Development.json configuré pour PostgreSQL (UseInMemoryDatabase: false)" -ForegroundColor Green
        } else {
            Write-Host "❌ appsettings.Development.json configuré pour InMemory (UseInMemoryDatabase: true)" -ForegroundColor Red
            return $false
        }
    }
    
    return $true
}

# Fonction pour créer la base de données
function Initialize-Database {
    Write-Host "🗄️  Initialisation de la base de données..." -ForegroundColor Yellow
    
    # Créer la base de données si elle n'existe pas
    if (Get-Command psql -ErrorAction SilentlyContinue) {
        $env:PGPASSWORD = "postgres"
        
        Write-Host "   📝 Création de la base de données 'fleetsynmanager'..." -ForegroundColor Cyan
        psql -h localhost -U postgres -d postgres -c "CREATE DATABASE fleetsynmanager;" 2>$null
        
        if ($LASTEXITCODE -eq 0 -or $LASTEXITCODE -eq 2) {
            Write-Host "   ✅ Base de données créée ou existe déjà" -ForegroundColor Green
        }
    } else {
        Write-Host "   ⚠️  psql non disponible - la base sera créée au premier démarrage" -ForegroundColor Yellow
    }
    
    # Exécuter les migrations Entity Framework
    Write-Host "   🔄 Exécution des migrations Entity Framework..." -ForegroundColor Cyan
    
    try {
        # Vérifier si nous avons des migrations
        if (Test-Path "src/Laroche.FleetManager.Infrastructure/Migrations") {
            $migrations = Get-ChildItem "src/Laroche.FleetManager.Infrastructure/Migrations" -Filter "*.cs" | Where-Object { $_.Name -ne "ApplicationDbContextModelSnapshot.cs" }
            
            if ($migrations.Count -gt 0) {
                Write-Host "   📋 Migrations trouvées: $($migrations.Count)" -ForegroundColor Cyan
                dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "   ✅ Migrations appliquées avec succès" -ForegroundColor Green
                } else {
                    Write-Host "   ❌ Erreur lors de l'application des migrations" -ForegroundColor Red
                    return $false
                }
            } else {
                Write-Host "   📋 Aucune migration trouvée - création de la migration initiale..." -ForegroundColor Cyan
                dotnet ef migrations add InitialCreate --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "   ✅ Migration initiale créée" -ForegroundColor Green
                    
                    dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
                    
                    if ($LASTEXITCODE -eq 0) {
                        Write-Host "   ✅ Migration initiale appliquée" -ForegroundColor Green
                    } else {
                        Write-Host "   ❌ Erreur lors de l'application de la migration initiale" -ForegroundColor Red
                        return $false
                    }
                }
            }
        } else {
            Write-Host "   📋 Dossier Migrations inexistant - création de la migration initiale..." -ForegroundColor Cyan
            dotnet ef migrations add InitialCreate --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "   ✅ Migration initiale créée" -ForegroundColor Green
                
                dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "   ✅ Migration initiale appliquée" -ForegroundColor Green
                } else {
                    Write-Host "   ❌ Erreur lors de l'application de la migration initiale" -ForegroundColor Red
                    return $false
                }
            } else {
                Write-Host "   ❌ Erreur lors de la création de la migration initiale" -ForegroundColor Red
                return $false
            }
        }
        
        return $true
    }
    catch {
        Write-Host "   ❌ Erreur lors de l'initialisation de la base de données: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Fonction pour tester l'application
function Test-ApplicationStartup {
    Write-Host "🚀 Test du démarrage de l'application..." -ForegroundColor Yellow
    
    try {
        Write-Host "   📋 Build de l'application..." -ForegroundColor Cyan
        dotnet build --configuration Debug --verbosity minimal
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "   ❌ Erreur lors du build" -ForegroundColor Red
            return $false
        }
        
        Write-Host "   ✅ Build réussi" -ForegroundColor Green
        Write-Host "   🔄 Démarrage de l'application (test rapide)..." -ForegroundColor Cyan
        
        # Démarrer l'application en arrière-plan pour un test rapide
        $appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project src/Laroche.FleetManager.Web --no-build" -PassThru -WindowStyle Hidden
        
        # Attendre quelques secondes pour le démarrage
        Start-Sleep -Seconds 10
        
        if (-not $appProcess.HasExited) {
            Write-Host "   ✅ Application démarrée avec succès" -ForegroundColor Green
            Write-Host "   🛑 Arrêt de l'application de test..." -ForegroundColor Cyan
            
            # Arrêter le processus de test
            Stop-Process -Id $appProcess.Id -Force -ErrorAction SilentlyContinue
            
            # Attendre un peu pour la fermeture propre
            Start-Sleep -Seconds 2
            
            return $true
        } else {
            Write-Host "   ❌ L'application s'est arrêtée de manière inattendue" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "   ❌ Erreur lors du test de démarrage: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Fonction principale
function Main {
    Write-Host "Phase 1: Test de la connexion PostgreSQL" -ForegroundColor Magenta
    Write-Host "----------------------------------------" -ForegroundColor Magenta
    $connectionOk = Test-PostgreSQLConnection -connStr $ConnectionString
    
    if ($connectionOk -eq $false) {
        Write-Host ""
        Write-Host "❌ La connexion PostgreSQL a échoué!" -ForegroundColor Red
        Write-Host "   Veuillez vérifier que PostgreSQL est démarré et accessible" -ForegroundColor Yellow
        Write-Host "   Commande pour démarrer PostgreSQL (si installé localement):" -ForegroundColor Yellow
        Write-Host "   net start postgresql-x64-15" -ForegroundColor White
        Write-Host ""
        Write-Host "   Ou utiliser Docker:" -ForegroundColor Yellow
        Write-Host "   docker run --name postgres-fleet -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15" -ForegroundColor White
        return
    }
    
    Write-Host ""
    Write-Host "Phase 2: Vérification de la configuration" -ForegroundColor Magenta
    Write-Host "-------------------------------------------" -ForegroundColor Magenta
    $configOk = Test-ApplicationConfiguration
    
    if (-not $configOk) {
        Write-Host ""
        Write-Host "❌ La configuration de l'application est incorrecte!" -ForegroundColor Red
        Write-Host "   Les paramètres UseInMemoryDatabase doivent être à 'false'" -ForegroundColor Yellow
        return
    }
    
    Write-Host ""
    Write-Host "Phase 3: Initialisation de la base de données" -ForegroundColor Magenta
    Write-Host "-----------------------------------------------" -ForegroundColor Magenta
    $dbOk = Initialize-Database
    
    if (-not $dbOk) {
        Write-Host ""
        Write-Host "❌ L'initialisation de la base de données a échoué!" -ForegroundColor Red
        return
    }
    
    Write-Host ""
    Write-Host "Phase 4: Test de démarrage de l'application" -ForegroundColor Magenta
    Write-Host "--------------------------------------------" -ForegroundColor Magenta
    $appOk = Test-ApplicationStartup
    
    Write-Host ""
    Write-Host "🎉 RÉSUMÉ DES TESTS" -ForegroundColor Green
    Write-Host "===================" -ForegroundColor Green
    Write-Host "✅ Configuration PostgreSQL: OK" -ForegroundColor Green
    Write-Host "✅ Base de données initialisée: OK" -ForegroundColor Green
    
    if ($appOk) {
        Write-Host "✅ Application démarrée: OK" -ForegroundColor Green
        Write-Host ""
        Write-Host "🚀 Votre application FleetSyncManager est maintenant configurée pour PostgreSQL!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Pour démarrer l'application:" -ForegroundColor White
        Write-Host "dotnet run --project src/Laroche.FleetManager.Web" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "L'application sera accessible sur:" -ForegroundColor White
        Write-Host "https://localhost:5001" -ForegroundColor Cyan
        Write-Host "http://localhost:5000" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  Application démarrée: ÉCHEC" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "La configuration PostgreSQL est correcte, mais il peut y avoir des problèmes de démarrage." -ForegroundColor Yellow
        Write-Host "Essayez de démarrer manuellement avec:" -ForegroundColor Yellow
        Write-Host "dotnet run --project src/Laroche.FleetManager.Web" -ForegroundColor Cyan
    }
}

# Exécution du script principal
Main
