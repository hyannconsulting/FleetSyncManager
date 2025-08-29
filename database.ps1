# Script de gestion de la base de données PostgreSQL pour FleetSyncManager
param(
    [string]$Action = "start",
    [switch]$Help
)

$DockerComposeFile = "docker-compose.dev.yml"
$ProjectName = "fleetmanager"

function Show-Help {
    Write-Host "🗃️  Script de Gestion Base de Données - FleetSyncManager" -ForegroundColor Cyan
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\database.ps1 [ACTION]" -ForegroundColor White
    Write-Host ""
    Write-Host "Actions disponibles:" -ForegroundColor Yellow
    Write-Host "  start     - Démarrer PostgreSQL et PgAdmin (par défaut)" -ForegroundColor Green
    Write-Host "  stop      - Arrêter tous les services" -ForegroundColor Red
    Write-Host "  restart   - Redémarrer tous les services" -ForegroundColor Blue
    Write-Host "  reset     - Supprimer et recréer la base de données" -ForegroundColor Magenta
    Write-Host "  logs      - Afficher les logs de PostgreSQL" -ForegroundColor Cyan
    Write-Host "  status    - Vérifier le statut des services" -ForegroundColor White
    Write-Host "  backup    - Créer une sauvegarde de la base" -ForegroundColor DarkYellow
    Write-Host "  help      - Afficher cette aide" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Informations de connexion:" -ForegroundColor Yellow
    Write-Host "  PostgreSQL: localhost:5432" -ForegroundColor White
    Write-Host "  Database:   fleetmanager_dev" -ForegroundColor White
    Write-Host "  User:       fleetmanager" -ForegroundColor White
    Write-Host "  Password:   DevPassword123!" -ForegroundColor White
    Write-Host "  PgAdmin:    http://localhost:5050" -ForegroundColor White
    Write-Host "              admin@fleetmanager.local / AdminPassword123!" -ForegroundColor White
}

function Start-Database {
    Write-Host "🚀 Démarrage de PostgreSQL..." -ForegroundColor Green
    
    if (-not (Test-Path $DockerComposeFile)) {
        Write-Host "❌ Fichier $DockerComposeFile non trouvé!" -ForegroundColor Red
        return
    }

    try {
        docker-compose -f $DockerComposeFile -p $ProjectName up -d
        
        Write-Host "⏳ Attente du démarrage de PostgreSQL..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        # Vérification de la santé de PostgreSQL
        $attempts = 0
        $maxAttempts = 30
        
        do {
            $attempts++
            $health = docker inspect fleetmanager_postgres_dev --format='{{.State.Health.Status}}' 2>$null
            
            if ($health -eq "healthy") {
                Write-Host "✅ PostgreSQL est prêt!" -ForegroundColor Green
                break
            }
            
            Write-Host "⏳ Attente de PostgreSQL... ($attempts/$maxAttempts)" -ForegroundColor Yellow
            Start-Sleep -Seconds 2
            
        } while ($attempts -lt $maxAttempts)
        
        if ($attempts -eq $maxAttempts) {
            Write-Host "⚠️  PostgreSQL met du temps à démarrer. Vérifiez les logs." -ForegroundColor Yellow
        }
        
        Write-Host ""
        Write-Host "🎯 Services démarrés:" -ForegroundColor Cyan
        Write-Host "   📊 PostgreSQL: localhost:5432" -ForegroundColor White
        Write-Host "   🔧 PgAdmin:    http://localhost:5050" -ForegroundColor White
        Write-Host ""
        Write-Host "💡 Conseils:" -ForegroundColor Yellow
        Write-Host "   - Utilisez 'dotnet ef database update' pour appliquer les migrations" -ForegroundColor White
        Write-Host "   - Connectez-vous à PgAdmin avec admin@fleetmanager.local" -ForegroundColor White
        
    } catch {
        Write-Host "❌ Erreur lors du démarrage: $_" -ForegroundColor Red
    }
}

function Stop-Database {
    Write-Host "🛑 Arrêt des services..." -ForegroundColor Red
    
    try {
        docker-compose -f $DockerComposeFile -p $ProjectName down
        Write-Host "✅ Services arrêtés avec succès!" -ForegroundColor Green
    } catch {
        Write-Host "❌ Erreur lors de l'arrêt: $_" -ForegroundColor Red
    }
}

function Restart-Database {
    Write-Host "🔄 Redémarrage des services..." -ForegroundColor Blue
    Stop-Database
    Start-Sleep -Seconds 3
    Start-Database
}

function Reset-Database {
    Write-Host "⚠️  ATTENTION: Cette action va supprimer toutes les données!" -ForegroundColor Red
    $confirmation = Read-Host "Tapez 'CONFIRMER' pour continuer"
    
    if ($confirmation -ne "CONFIRMER") {
        Write-Host "❌ Action annulée." -ForegroundColor Yellow
        return
    }
    
    Write-Host "🗑️  Suppression des données..." -ForegroundColor Red
    
    try {
        docker-compose -f $DockerComposeFile -p $ProjectName down -v
        docker volume rm fleetmanager_postgres_data 2>$null
        docker volume rm fleetmanager_pgadmin_data 2>$null
        
        Write-Host "✅ Données supprimées!" -ForegroundColor Green
        Write-Host "🚀 Redémarrage avec une base de données vierge..." -ForegroundColor Green
        
        Start-Database
        
    } catch {
        Write-Host "❌ Erreur lors de la réinitialisation: $_" -ForegroundColor Red
    }
}

function Show-Logs {
    Write-Host "📋 Logs PostgreSQL:" -ForegroundColor Cyan
    docker-compose -f $DockerComposeFile -p $ProjectName logs postgres
}

function Show-Status {
    Write-Host "📊 Statut des services:" -ForegroundColor Cyan
    docker-compose -f $DockerComposeFile -p $ProjectName ps
}

function Backup-Database {
    $backupDir = "backups"
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupFile = "$backupDir/fleetmanager_backup_$timestamp.sql"
    
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
    }
    
    Write-Host "💾 Création de la sauvegarde..." -ForegroundColor Cyan
    
    try {
        docker exec fleetmanager_postgres_dev pg_dump -U fleetmanager fleetmanager_dev > $backupFile
        Write-Host "✅ Sauvegarde créée: $backupFile" -ForegroundColor Green
    } catch {
        Write-Host "❌ Erreur lors de la sauvegarde: $_" -ForegroundColor Red
    }
}

# Script principal
if ($Help) {
    Show-Help
    exit 0
}

switch ($Action.ToLower()) {
    "start" { Start-Database }
    "stop" { Stop-Database }
    "restart" { Restart-Database }
    "reset" { Reset-Database }
    "logs" { Show-Logs }
    "status" { Show-Status }
    "backup" { Backup-Database }
    "help" { Show-Help }
    default {
        Write-Host "❌ Action inconnue: $Action" -ForegroundColor Red
        Write-Host "Utilisez '.\database.ps1 help' pour voir les actions disponibles." -ForegroundColor Yellow
    }
}
