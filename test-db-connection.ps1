# Test de connexion et diagnostic PostgreSQL
Write-Host "=== TEST DE CONNEXION POSTGRESQL FLEETSYNCMANAGER ===" -ForegroundColor Green

# Paramètres de connexion actuels
$dbHost = "localhost"
$port = "5432"
$database = "fleetmanager_dev"
$username = "fleetmanager"
$password = "DevPassword123!"

Write-Host "`n1. Test de connexion PostgreSQL..." -ForegroundColor Yellow

# Test de connexion
$env:PGPASSWORD = $password
$connectionTest = docker exec ce79cc90b4b2 psql -U $username -d $database -c "SELECT current_database(), current_user, version();" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Connexion PostgreSQL: REUSSIE" -ForegroundColor Green
} else {
    Write-Host "   Connexion PostgreSQL: ECHEC" -ForegroundColor Red
    Write-Host "   Verifiez les parametres de connexion" -ForegroundColor Yellow
    exit 1
}

Write-Host "`n2. Verification des tables existantes..." -ForegroundColor Yellow

# Lister toutes les tables
$tablesList = docker exec ce79cc90b4b2 psql -U $username -d $database -c "\dt" 2>$null

if ($tablesList -match "Did not find any relations") {
    Write-Host "   Aucune table trouvee dans la base de donnees" -ForegroundColor Red
    Write-Host "   Les migrations n'ont peut-etre pas ete appliquees correctement" -ForegroundColor Yellow
} else {
    Write-Host "   Tables trouvees:" -ForegroundColor Green
    docker exec ce79cc90b4b2 psql -U $username -d $database -c "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' ORDER BY table_name;"
}

Write-Host "`n3. Verification de l'historique des migrations..." -ForegroundColor Yellow

# Vérifier la table des migrations
$migrationsCheck = docker exec ce79cc90b4b2 psql -U $username -d $database -c "SELECT COUNT(*) FROM \`"__EFMigrationsHistory\`";" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Table des migrations trouvee" -ForegroundColor Green
    docker exec ce79cc90b4b2 psql -U $username -d $database -c "SELECT \`"MigrationId\`", \`"ProductVersion\`" FROM \`"__EFMigrationsHistory\`" ORDER BY \`"MigrationId\`";"
} else {
    Write-Host "   Table des migrations non trouvee" -ForegroundColor Red
    Write-Host "   Les migrations doivent etre re-appliquees" -ForegroundColor Yellow
}

Write-Host "`n4. Informations de la base de donnees..." -ForegroundColor Yellow

# Informations générales
docker exec ce79cc90b4b2 psql -U $username -d $database -c "
SELECT 
    'Database' as Info, current_database() as Value
UNION ALL
SELECT 
    'User' as Info, current_user as Value
UNION ALL
SELECT 
    'Schema' as Info, current_schema() as Value
UNION ALL
SELECT 
    'Version' as Info, version() as Value;
"

Write-Host "`n5. Test de creation de table..." -ForegroundColor Yellow

# Test rapide de création/suppression de table
$testTable = docker exec ce79cc90b4b2 psql -U $username -d $database -c "
CREATE TABLE test_table (id SERIAL PRIMARY KEY, name VARCHAR(50));
INSERT INTO test_table (name) VALUES ('Test PostgreSQL');
SELECT * FROM test_table;
DROP TABLE test_table;
" 2>$null

if ($LASTEXITCODE -eq 0) {
    Write-Host "   Test de creation de table: REUSSI" -ForegroundColor Green
    Write-Host "   PostgreSQL fonctionne correctement" -ForegroundColor Green
} else {
    Write-Host "   Test de creation de table: ECHEC" -ForegroundColor Red
    Write-Host "   Probleme avec les permissions ou la base de donnees" -ForegroundColor Yellow
}

Write-Host "`n=== RESUME ===" -ForegroundColor Magenta
Write-Host "Base de donnees: $database" -ForegroundColor White
Write-Host "Utilisateur: $username" -ForegroundColor White
Write-Host "Host: $dbHost" -ForegroundColor White
Write-Host "Port: $port" -ForegroundColor White

Write-Host "`n=== COMMANDES POUR SE CONNECTER ===" -ForegroundColor Cyan
Write-Host "Via Docker:" -ForegroundColor White
Write-Host "docker exec -it ce79cc90b4b2 psql -U $username -d $database" -ForegroundColor Green

Write-Host "`nVia psql local (si installe):" -ForegroundColor White
Write-Host "`$env:PGPASSWORD='$password'; psql -h $dbHost -p $port -U $username -d $database" -ForegroundColor Green

Write-Host "`nURL de connexion:" -ForegroundColor White
Write-Host "postgresql://$username`:$password@$dbHost`:$port/$database" -ForegroundColor Green

Write-Host "`n=== REQUETES UTILES ===" -ForegroundColor Cyan
Write-Host "Lister les tables:" -ForegroundColor White
Write-Host "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';" -ForegroundColor Green

Write-Host "`nVoir les migrations:" -ForegroundColor White
Write-Host "SELECT * FROM \`"__EFMigrationsHistory\`";" -ForegroundColor Green

Write-Host "`n=== SI LES MIGRATIONS SONT MANQUANTES ===" -ForegroundColor Yellow
Write-Host "Re-appliquer les migrations:" -ForegroundColor White
Write-Host "dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web" -ForegroundColor Green

Write-Host "`nOu forcer une nouvelle migration:" -ForegroundColor White
Write-Host "dotnet ef migrations add ForceMigration --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web" -ForegroundColor Green
Write-Host "dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web" -ForegroundColor Green
