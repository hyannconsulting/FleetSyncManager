# Script pour se connecter à PostgreSQL et consulter les données FleetSyncManager
Write-Host "Guide de connexion PostgreSQL - FleetSyncManager" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Récupération des paramètres de connexion depuis appsettings.json
$configPath = "src\Laroche.FleetManager.Web\appsettings.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    $connectionString = $config.ConnectionStrings.DefaultConnection
    
    # Parse de la chaîne de connexion
    $connParams = @{}
    $connectionString -split ';' | ForEach-Object {
        if ($_ -match '(.+?)=(.+)') {
            $connParams[$matches[1].Trim()] = $matches[2].Trim()
        }
    }
    
    Write-Host "`nParametres de connexion detectes:" -ForegroundColor Yellow
    Write-Host "  Host: $($connParams['Host'])" -ForegroundColor White
    Write-Host "  Port: $($connParams['Port'])" -ForegroundColor White
    Write-Host "  Database: $($connParams['Database'])" -ForegroundColor White
    Write-Host "  Username: $($connParams['Username'])" -ForegroundColor White
    Write-Host "  Password: [MASQUE]" -ForegroundColor White
    
    # Variables pour les commandes
    $dbHost = $connParams['Host']
    $port = $connParams['Port']
    $database = $connParams['Database']
    $username = $connParams['Username']
    $password = $connParams['Password']
} else {
    Write-Host "Fichier de configuration non trouve!" -ForegroundColor Red
    $dbHost = "localhost"
    $port = "5432"
    $database = "fleetsyncmanager"
    $username = "fleetsync_user"
    $password = "FleetSync123!"
    
    Write-Host "Utilisation des parametres par defaut:" -ForegroundColor Yellow
    Write-Host "  Host: $dbHost" -ForegroundColor White
    Write-Host "  Port: $port" -ForegroundColor White
    Write-Host "  Database: $database" -ForegroundColor White
    Write-Host "  Username: $username" -ForegroundColor White
}

Write-Host "`n=== METHODES DE CONNEXION ===" -ForegroundColor Cyan

Write-Host "`n1. CONNEXION VIA PSQL (Ligne de commande)" -ForegroundColor Yellow
Write-Host "   Commande pour se connecter:" -ForegroundColor White
Write-Host "   psql -h $dbHost -p $port -U $username -d $database" -ForegroundColor Green
Write-Host ""
Write-Host "   Ou en une ligne avec mot de passe:" -ForegroundColor White
Write-Host "   `$env:PGPASSWORD='$password'; psql -h $dbHost -p $port -U $username -d $database" -ForegroundColor Green

Write-Host "`n2. CONNEXION VIA PGADMIN (Interface graphique)" -ForegroundColor Yellow
Write-Host "   Parametres de connexion pgAdmin:" -ForegroundColor White
Write-Host "   - Host/Address: $dbHost" -ForegroundColor Cyan
Write-Host "   - Port: $port" -ForegroundColor Cyan
Write-Host "   - Database: $database" -ForegroundColor Cyan
Write-Host "   - Username: $username" -ForegroundColor Cyan
Write-Host "   - Password: $password" -ForegroundColor Cyan

Write-Host "`n3. CONNEXION VIA URL DE CONNEXION" -ForegroundColor Yellow
$connectionUrl = "postgresql://$username`:$password@$dbHost`:$port/$database"
Write-Host "   URL de connexion complete:" -ForegroundColor White
Write-Host "   $connectionUrl" -ForegroundColor Green

Write-Host "`n=== REQUETES SQL UTILES ===" -ForegroundColor Cyan

Write-Host "`n1. LISTER TOUTES LES TABLES:" -ForegroundColor Yellow
Write-Host "   SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';" -ForegroundColor Green

Write-Host "`n2. VOIR LA STRUCTURE D'UNE TABLE (exemple: Vehicles):" -ForegroundColor Yellow
Write-Host "   \d `"Vehicles`"" -ForegroundColor Green
Write-Host "   -- ou --" -ForegroundColor White
Write-Host "   SELECT column_name, data_type, is_nullable FROM information_schema.columns WHERE table_name = 'Vehicles';" -ForegroundColor Green

Write-Host "`n3. CONSULTER LES DONNEES:" -ForegroundColor Yellow
Write-Host "   -- Tous les vehicules" -ForegroundColor White
Write-Host "   SELECT * FROM `"Vehicles`" LIMIT 10;" -ForegroundColor Green
Write-Host ""
Write-Host "   -- Tous les conducteurs" -ForegroundColor White
Write-Host "   SELECT * FROM `"Drivers`" LIMIT 10;" -ForegroundColor Green
Write-Host ""
Write-Host "   -- Incidents recents" -ForegroundColor White
Write-Host "   SELECT * FROM `"Incidents`" ORDER BY `"CreatedAt`" DESC LIMIT 10;" -ForegroundColor Green

Write-Host "`n4. STATISTIQUES:" -ForegroundColor Yellow
Write-Host "   -- Nombre total de vehicules" -ForegroundColor White
Write-Host "   SELECT COUNT(*) as TotalVehicles FROM `"Vehicles`";" -ForegroundColor Green
Write-Host ""
Write-Host "   -- Vehicules par statut" -ForegroundColor White
Write-Host "   SELECT `"Status`", COUNT(*) as Count FROM `"Vehicles`" GROUP BY `"Status`";" -ForegroundColor Green

Write-Host "`n5. JOINTURES UTILES:" -ForegroundColor Yellow
Write-Host "   -- Vehicules avec leurs conducteurs assignes" -ForegroundColor White
Write-Host "   SELECT v.`"LicensePlate`", v.`"Brand`", d.`"FirstName`", d.`"LastName`"" -ForegroundColor Green
Write-Host "   FROM `"Vehicles`" v" -ForegroundColor Green
Write-Host "   LEFT JOIN `"Drivers`" d ON v.`"AssignedDriverId`" = d.`"Id`";" -ForegroundColor Green

Write-Host "`n=== COMMANDES D'ADMINISTRATION ===" -ForegroundColor Cyan

Write-Host "`n1. VOIR LES MIGRATIONS APPLIQUEES:" -ForegroundColor Yellow
Write-Host "   SELECT * FROM `"__EFMigrationsHistory`" ORDER BY `"MigrationId`";" -ForegroundColor Green

Write-Host "`n2. TAILLE DE LA BASE DE DONNEES:" -ForegroundColor Yellow
Write-Host "   SELECT pg_size_pretty(pg_database_size('$database'));" -ForegroundColor Green

Write-Host "`n3. CONNEXIONS ACTIVES:" -ForegroundColor Yellow
Write-Host "   SELECT count(*) FROM pg_stat_activity WHERE datname = '$database';" -ForegroundColor Green

Write-Host "`n=== DEMARRAGE RAPIDE ===" -ForegroundColor Magenta
Write-Host "Si PostgreSQL n'est pas encore demarre:" -ForegroundColor Yellow
Write-Host "docker run --name postgres-fleet -e POSTGRES_PASSWORD=$password -p $port`:5432 -d postgres:15" -ForegroundColor Green

Write-Host "`nPour se connecter directement:" -ForegroundColor Yellow
Write-Host "`$env:PGPASSWORD='$password'; psql -h $dbHost -p $port -U $username -d $database" -ForegroundColor Green

Write-Host "`n=== OUTILS RECOMMANDES ===" -ForegroundColor Magenta
Write-Host "1. pgAdmin 4 - Interface graphique complete" -ForegroundColor White
Write-Host "   Telechargement: https://www.pgadmin.org/download/" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. DBeaver - Client universel gratuit" -ForegroundColor White
Write-Host "   Telechargement: https://dbeaver.io/download/" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. Azure Data Studio - Avec extension PostgreSQL" -ForegroundColor White
Write-Host "   Telechargement: https://docs.microsoft.com/en-us/sql/azure-data-studio/" -ForegroundColor Cyan
