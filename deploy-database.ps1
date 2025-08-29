# FleetSyncManager - Script de Deploiement Base de Donnees PostgreSQL
# Configure pour utiliser PostgreSQL par defaut

param(
    [string]$ConnectionString = "Host=localhost;Port=5432;Database=fleetmanager_dev;Username=fleetmanager;Password=DevPassword123!",
    [switch]$Help
)

function Show-Help {
    Write-Host "============================================================================="
    Write-Host "FleetSyncManager - Script de Deploiement PostgreSQL"
    Write-Host "============================================================================="
    Write-Host ""
    Write-Host "UTILISATION:"
    Write-Host "  .\deploy-database.ps1 [PARAMETRES]"
    Write-Host ""
    Write-Host "PARAMETRES:"
    Write-Host "  -ConnectionString    Chaine de connexion PostgreSQL"
    Write-Host "                      Defaut: Host=localhost;Port=5432;Database=fleetmanager_dev;Username=fleetmanager;Password=DevPassword123!"
    Write-Host "  -Help               Afficher cette aide"
    Write-Host ""
    Write-Host "EXEMPLE:"
    Write-Host "  .\deploy-database.ps1"
    Write-Host ""
    Write-Host "============================================================================="
}

function Execute-MigrationScript {
    param([string]$ConnectionString)
    
    Write-Host "=============================================================================" -ForegroundColor Cyan
    Write-Host "EXECUTION DES MIGRATIONS POSTGRESQL" -ForegroundColor Green
    Write-Host "=============================================================================" -ForegroundColor Cyan
    
    if (-not (Test-Path "migration-script.sql")) {
        Write-Host "[ERREUR] Fichier migration-script.sql introuvable" -ForegroundColor Red
        return $false
    }
    
    try {
        # Extraction des parametres de connexion
        $connParams = @{}
        $ConnectionString.Split(';') | ForEach-Object {
            if ($_ -and $_.Contains('=')) {
                $key, $value = $_.Split('=', 2)
                $connParams[$key.Trim()] = $value.Trim()
            }
        }
        
        Write-Host "[INFO] Base de donnees cible: $($connParams['Database'])" -ForegroundColor Yellow
        Write-Host "[INFO] Serveur: $($connParams['Host']):$($connParams['Port'])" -ForegroundColor Yellow
        Write-Host "[INFO] Utilisateur: $($connParams['Username'])" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "[INFO] Execution du script de migration SQL..." -ForegroundColor Yellow
        
        # Test de la commande psql
        $psqlPath = Get-Command psql -ErrorAction SilentlyContinue
        if (-not $psqlPath) {
            Write-Host "[ERREUR] psql n'est pas installe ou pas dans le PATH" -ForegroundColor Red
            Write-Host "[INFO] Installez PostgreSQL client ou ajoutez psql au PATH" -ForegroundColor Yellow
            return $false
        }
        
        $env:PGPASSWORD = $connParams['Password']
        
        # Execution du script
        $result = Get-Content "migration-script.sql" | & psql -h $connParams['Host'] -p $connParams['Port'] -U $connParams['Username'] -d $connParams['Database'] 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[SUCCES] Script SQL execute avec succes" -ForegroundColor Green
            Write-Host "[INFO] Base de donnees PostgreSQL prete pour FleetSyncManager" -ForegroundColor Green
            return $true
        } else {
            Write-Host "[ERREUR] Erreur lors de l'execution du script SQL" -ForegroundColor Red
            Write-Host $result
            return $false
        }
    }
    catch {
        Write-Host "[ERREUR] Exception: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-PostgreSQLConnection {
    param([string]$ConnectionString)
    
    Write-Host "[INFO] Test de connexion PostgreSQL..." -ForegroundColor Yellow
    
    try {
        # Extraction des parametres de connexion
        $connParams = @{}
        $ConnectionString.Split(';') | ForEach-Object {
            if ($_ -and $_.Contains('=')) {
                $key, $value = $_.Split('=', 2)
                $connParams[$key.Trim()] = $value.Trim()
            }
        }
        
        # Test avec psql si disponible
        $psqlPath = Get-Command psql -ErrorAction SilentlyContinue
        if ($psqlPath) {
            $env:PGPASSWORD = $connParams['Password']
            $testResult = & psql -h $connParams['Host'] -p $connParams['Port'] -U $connParams['Username'] -d $connParams['Database'] -c "SELECT version();" 2>$null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "[SUCCES] Connexion PostgreSQL etablie" -ForegroundColor Green
                return $true
            }
        }
        
        Write-Host "[AVERTISSEMENT] Impossible de tester la connexion PostgreSQL" -ForegroundColor Yellow
        Write-Host "[INFO] Verifiez que PostgreSQL est demarre et que la base de donnees existe" -ForegroundColor Yellow
        return $true # Continue quand même
    }
    catch {
        Write-Host "[AVERTISSEMENT] Erreur lors du test de connexion: $($_.Exception.Message)" -ForegroundColor Yellow
        return $true # Continue quand même
    }
}

# SCRIPT PRINCIPAL
if ($Help) {
    Show-Help
    exit 0
}

Write-Host "=============================================================================" -ForegroundColor Cyan
Write-Host "FLEETSYNSMANAGER - CONFIGURATION POSTGRESQL" -ForegroundColor Green
Write-Host "=============================================================================" -ForegroundColor Cyan

Write-Host "[INFO] Configuration PostgreSQL comme base de donnees par defaut" -ForegroundColor Green
Write-Host ""

# Test de la connexion
Test-PostgreSQLConnection $ConnectionString

# Execution des migrations
$success = Execute-MigrationScript $ConnectionString

Write-Host ""
if ($success) {
    Write-Host "[SUCCES] Configuration PostgreSQL terminee avec succes" -ForegroundColor Green
    Write-Host ""
    Write-Host "PROCHAINES ETAPES:" -ForegroundColor Yellow
    Write-Host "1. Verifiez que PostgreSQL est demarre" -ForegroundColor White
    Write-Host "2. Creez la base de donnees si necessaire:" -ForegroundColor White
    Write-Host "   createdb -U postgres fleetmanager_dev" -ForegroundColor Gray
    Write-Host "3. Demarrez l'application:" -ForegroundColor White
    Write-Host "   dotnet run --project src/Laroche.FleetManager.Web" -ForegroundColor Gray
    Write-Host ""
    Write-Host "[INFO] L'application utilisera desormais PostgreSQL au lieu de InMemory" -ForegroundColor Green
} else {
    Write-Host "[ERREUR] Echec de la configuration PostgreSQL" -ForegroundColor Red
    Write-Host ""
    Write-Host "VERIFICATION:" -ForegroundColor Yellow
    Write-Host "- PostgreSQL est-il installe et demarre ?" -ForegroundColor White
    Write-Host "- La base de donnees existe-t-elle ?" -ForegroundColor White
    Write-Host "- Les parametres de connexion sont-ils corrects ?" -ForegroundColor White
}

Write-Host ""
Write-Host "Script termine." -ForegroundColor Green
