# Guide pratique - Connexion PostgreSQL FleetSyncManager

## 🔗 CONNEXION RAPIDE

### Paramètres de connexion
- **Host:** localhost  
- **Port:** 5432
- **Database:** fleetsyncmanager
- **Username:** fleetsync_user
- **Password:** FleetSync123!

### Démarrage PostgreSQL (si pas encore fait)
```powershell
docker run --name postgres-fleet -e POSTGRES_PASSWORD=FleetSync123! -p 5432:5432 -d postgres:15
```

### Connexion ligne de commande (psql)
```powershell
$env:PGPASSWORD='FleetSync123!'; psql -h localhost -p 5432 -U fleetsync_user -d fleetsyncmanager
```

### URL de connexion complète
```
postgresql://fleetsync_user:FleetSync123!@localhost:5432/fleetsyncmanager
```

## 🔍 REQUÊTES ESSENTIELLES

### Lister toutes les tables
```sql
SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';
```

### Voir les données principales
```sql
-- Tous les véhicules
SELECT * FROM "Vehicles" LIMIT 10;

-- Tous les conducteurs  
SELECT * FROM "Drivers" LIMIT 10;

-- Incidents récents
SELECT * FROM "Incidents" ORDER BY "CreatedAt" DESC LIMIT 10;

-- Maintenances programmées
SELECT * FROM "MaintenanceRecords" ORDER BY "ScheduledDate" DESC LIMIT 10;
```

### Statistiques utiles
```sql
-- Nombre total de véhicules
SELECT COUNT(*) as TotalVehicles FROM "Vehicles";

-- Véhicules par statut
SELECT "Status", COUNT(*) as Count FROM "Vehicles" GROUP BY "Status";

-- Incidents par mois
SELECT DATE_TRUNC('month', "CreatedAt") as Month, COUNT(*) as IncidentCount 
FROM "Incidents" 
GROUP BY DATE_TRUNC('month', "CreatedAt") 
ORDER BY Month DESC;
```

### Jointures importantes
```sql
-- Véhicules avec conducteurs assignés
SELECT 
    v."LicensePlate", 
    v."Brand", 
    v."Model",
    d."FirstName", 
    d."LastName"
FROM "Vehicles" v
LEFT JOIN "Drivers" d ON v."AssignedDriverId" = d."Id";

-- Incidents avec détails véhicule et conducteur
SELECT 
    i."Description",
    i."CreatedAt",
    v."LicensePlate",
    d."FirstName" || ' ' || d."LastName" as DriverName
FROM "Incidents" i
JOIN "Vehicles" v ON i."VehicleId" = v."Id"
LEFT JOIN "Drivers" d ON v."AssignedDriverId" = d."Id"
ORDER BY i."CreatedAt" DESC;
```

## 🛠️ OUTILS RECOMMANDÉS

### 1. pgAdmin 4 (Interface graphique)
- **Téléchargement:** https://www.pgadmin.org/download/
- **Configuration:**
  - Host: localhost
  - Port: 5432  
  - Database: fleetsyncmanager
  - Username: fleetsync_user
  - Password: FleetSync123!

### 2. DBeaver (Client universel gratuit)
- **Téléchargement:** https://dbeaver.io/download/
- **Type:** PostgreSQL
- **Mêmes paramètres de connexion**

### 3. VS Code avec extension PostgreSQL
- **Extension:** PostgreSQL par Chris Kolkman
- **Connection String:** postgresql://fleetsync_user:FleetSync123!@localhost:5432/fleetsyncmanager

## 📋 COMMANDES D'ADMINISTRATION

### Voir les migrations appliquées
```sql
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
```

### Taille de la base de données
```sql
SELECT pg_size_pretty(pg_database_size('fleetsyncmanager'));
```

### Connexions actives
```sql
SELECT count(*) FROM pg_stat_activity WHERE datname = 'fleetsyncmanager';
```

### Structure d'une table
```sql
-- Via psql
\d "Vehicles"

-- Via SQL standard
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'Vehicles';
```

## 🚀 DÉMARRAGE COMPLET

1. **Démarrer PostgreSQL:**
   ```powershell
   .\start-postgresql.ps1
   ```

2. **Se connecter:**
   ```powershell
   .\connect-postgresql.ps1
   ```

3. **Lancer l'application:**
   ```powershell
   dotnet run --project src/Laroche.FleetManager.Web
   ```

## 🔧 DÉPANNAGE

### PostgreSQL ne démarre pas
```powershell
# Voir les logs
docker logs postgres-fleet

# Redémarrer le conteneur
docker restart postgres-fleet
```

### Erreur de connexion
```powershell
# Vérifier que PostgreSQL écoute
docker exec postgres-fleet pg_isready -h localhost -p 5432

# Tester la connexion
$env:PGPASSWORD='FleetSync123!'; psql -h localhost -p 5432 -U postgres -d postgres -c "SELECT version();"
```

### Recréer la base de données
```sql
-- Se connecter en tant que postgres
DROP DATABASE IF EXISTS "fleetsyncmanager";
CREATE DATABASE "fleetsyncmanager" OWNER "fleetsync_user";
```

Puis relancer les migrations :
```powershell
dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
```
