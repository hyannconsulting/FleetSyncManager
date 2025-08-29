# Script pour démarrer PostgreSQL et initialiser la base FleetSyncManager
Write-Host "Demarrage et initialisation PostgreSQL - FleetSyncManager" -ForegroundColor Green
Write-Host "=======================================================" -ForegroundColor Green

# Paramètres de connexion
$dbHost = "localhost"
$port = "5432"
$database = "fleetsyncmanager"
$username = "fleetsync_user" 
$password = "FleetSync123!"
$containerName = "postgres-fleet"

Write-Host "`n1. Verification si PostgreSQL est deja en cours d'execution..." -ForegroundColor Yellow

# Vérifier si le conteneur Docker existe déjà
$existingContainer = docker ps -a --filter "name=$containerName" --format "{{.Names}}" 2>$null

if ($existingContainer) {
    Write-Host "   Conteneur '$containerName' trouve" -ForegroundColor Green
    
    # Vérifier si il est en cours d'execution
    $runningContainer = docker ps --filter "name=$containerName" --format "{{.Names}}" 2>$null
    
    if ($runningContainer) {
        Write-Host "   PostgreSQL est deja en cours d'execution" -ForegroundColor Green
    } else {
        Write-Host "   Demarrage du conteneur existant..." -ForegroundColor Cyan
        docker start $containerName
        Start-Sleep -Seconds 3
        Write-Host "   PostgreSQL demarre" -ForegroundColor Green
    }
} else {
    Write-Host "   Aucun conteneur PostgreSQL trouve" -ForegroundColor Yellow
    Write-Host "`n2. Creation et demarrage d'un nouveau conteneur PostgreSQL..." -ForegroundColor Yellow
    
    # Créer et démarrer le conteneur PostgreSQL
    docker run --name $containerName -e POSTGRES_PASSWORD=$password -p ${port}:5432 -d postgres:15
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   Conteneur PostgreSQL cree et demarre avec succes" -ForegroundColor Green
        Write-Host "   Attente du demarrage complet..." -ForegroundColor Cyan
        Start-Sleep -Seconds 10
    } else {
        Write-Host "   Erreur lors de la creation du conteneur" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n3. Test de connexion PostgreSQL..." -ForegroundColor Yellow

# Test de connexion avec psql si disponible
if (Get-Command psql -ErrorAction SilentlyContinue) {
    $env:PGPASSWORD = $password
    
    # Test de connexion à la base par défaut
    $testConnection = psql -h $dbHost -p $port -U postgres -d postgres -c "SELECT 1;" 2>$null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   Connexion PostgreSQL reussie" -ForegroundColor Green
    } else {
        Write-Host "   Impossible de se connecter a PostgreSQL" -ForegroundColor Red
        Write-Host "   Verifiez que le service est demarre" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "`n4. Creation de l'utilisateur et de la base de donnees..." -ForegroundColor Yellow
    
    # Création de l'utilisateur si il n'existe pas
    Write-Host "   Creation de l'utilisateur '$username'..." -ForegroundColor Cyan
    $createUser = psql -h $dbHost -p $port -U postgres -d postgres -c "CREATE USER `"$username`" WITH PASSWORD '$password';" 2>$null
    
    # Création de la base de données si elle n'existe pas
    Write-Host "   Creation de la base de donnees '$database'..." -ForegroundColor Cyan
    $createDB = psql -h $dbHost -p $port -U postgres -d postgres -c "CREATE DATABASE `"$database`" OWNER `"$username`";" 2>$null
    
    # Attribution des privilèges
    Write-Host "   Attribution des privileges..." -ForegroundColor Cyan
    psql -h $dbHost -p $port -U postgres -d postgres -c "GRANT ALL PRIVILEGES ON DATABASE `"$database`" TO `"$username`";" 2>$null
    psql -h $dbHost -p $port -U postgres -d $database -c "GRANT ALL ON SCHEMA public TO `"$username`";" 2>$null
    
    Write-Host "   Base de donnees et utilisateur configures" -ForegroundColor Green
} else {
    Write-Host "   psql n'est pas installe - verification basique avec docker..." -ForegroundColor Yellow
    
    # Test basique avec Docker
    $dockerTest = docker exec $containerName pg_isready -h localhost -p 5432
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   PostgreSQL est pret" -ForegroundColor Green
    } else {
        Write-Host "   PostgreSQL ne repond pas" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n5. Application des migrations Entity Framework..." -ForegroundColor Yellow

# Appliquer les migrations EF
dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Migrations appliquees avec succes" -ForegroundColor Green
} else {
    Write-Host "   Erreur lors de l'application des migrations" -ForegroundColor Red
    Write-Host "   Verifiez la configuration de la base de donnees" -ForegroundColor Yellow
}

Write-Host "`n=== RESUME ===" -ForegroundColor Magenta
Write-Host "PostgreSQL: DEMARRE" -ForegroundColor Green
Write-Host "Base de donnees: $database" -ForegroundColor Green  
Write-Host "Utilisateur: $username" -ForegroundColor Green
Write-Host "Host: $dbHost" -ForegroundColor Green
Write-Host "Port: $port" -ForegroundColor Green

Write-Host "`n=== COMMANDES DE CONNEXION ===" -ForegroundColor Cyan
Write-Host "Ligne de commande:" -ForegroundColor White
Write-Host "`$env:PGPASSWORD='$password'; psql -h $dbHost -p $port -U $username -d $database" -ForegroundColor Green

Write-Host "`nURL de connexion:" -ForegroundColor White  
Write-Host "postgresql://$username`:$password@$dbHost`:$port/$database" -ForegroundColor Green

Write-Host "`n=== COMMANDES UTILES ===" -ForegroundColor Cyan
Write-Host "Arreter PostgreSQL:" -ForegroundColor White
Write-Host "docker stop $containerName" -ForegroundColor Yellow

Write-Host "`nRedemarrer PostgreSQL:" -ForegroundColor White
Write-Host "docker start $containerName" -ForegroundColor Yellow

Write-Host "`nSupprimer le conteneur:" -ForegroundColor White
Write-Host "docker rm -f $containerName" -ForegroundColor Yellow

Write-Host "`nVoir les logs PostgreSQL:" -ForegroundColor White
Write-Host "docker logs $containerName" -ForegroundColor Yellow

Write-Host "`nFleetSyncManager est pret a etre utilise avec PostgreSQL!" -ForegroundColor Green
