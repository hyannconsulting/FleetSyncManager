# 🗃️ Scripts de Migration Entity Framework - FleetSyncManager

Ce dossier contient les scripts de migration et de déploiement de base de données pour le projet FleetSyncManager.

## 📁 Fichiers Disponibles

### 1. `database-schema.sql`
**Description :** Script SQL complet pour PostgreSQL avec toutes les fonctionnalités avancées
- 📋 Schéma complet avec tables, index, contraintes
- 🔍 Vues utilitaires pour les rapports
- 📊 Données de test optionnelles
- 🛡️ Contraintes métier et règles de gestion

### 2. `migration-script.sql`
**Description :** Script de migration compatible Entity Framework Core
- 🔄 Format compatible avec EF Core 9.0.8
- 📝 Table `__EFMigrationsHistory` incluse
- 🎯 Optimisé pour l'intégration avec l'application .NET

### 3. `deploy-database.ps1`
**Description :** Script PowerShell automatisé pour le déploiement
- ⚙️ Installation automatique des outils EF Core
- 🔧 Test de connexion PostgreSQL
- 🚀 Déploiement en un clic
- 📊 Insertion de données de test

## 🚀 Utilisation Rapide

### Option 1 : Script PowerShell Automatisé (Recommandé)

```powershell
# Déploiement complet avec données de test
.\deploy-database.ps1 -CreateDatabase -RunMigrations -SeedData

# Seulement les migrations
.\deploy-database.ps1 -RunMigrations

# Environnement de production
.\deploy-database.ps1 -ConnectionString "Host=prod-server;Database=fleetsync_prod;Username=prod_user;Password=***" -Environment Production -RunMigrations
```

### Option 2 : Entity Framework CLI

```bash
# Installation des outils (si nécessaire)
dotnet tool install --global dotnet-ef

# Création d'une migration
dotnet ef migrations add InitialCreate --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web

# Application des migrations
dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
```

### Option 3 : SQL Direct

```bash
# Avec psql
psql -h localhost -U fleetsync_user -d fleetsync_dev -f migration-script.sql

# Ou depuis pgAdmin, DBeaver, etc.
```

## 🔧 Configuration

### Variables d'Environnement

```bash
# Chaîne de connexion par défaut
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=fleetsync_dev;Username=fleetsync_user;Password=dev_password"

# Environnement
ASPNETCORE_ENVIRONMENT=Development
```

### Configuration PostgreSQL

```bash
# Création de l'utilisateur PostgreSQL
createuser -P fleetsync_user

# Création de la base de données
createdb -O fleetsync_user fleetsync_dev
```

## 📊 Schéma de Base de Données

### Tables Principales

| Table | Description | Clés |
|-------|-------------|------|
| `Vehicles` | Véhicules de la flotte | LicensePlate (UK) |
| `Drivers` | Conducteurs | Email, LicenseNumber (UK) |
| `VehicleAssignments` | Assignations véhicule-conducteur | VehicleId, DriverId (FK) |
| `MaintenanceRecords` | Enregistrements de maintenance | VehicleId (FK) |
| `Incidents` | Incidents et sinistres | VehicleId, DriverId (FK) |
| `GpsTrackingRecords` | Suivi GPS temps réel | VehicleId (FK) |

### Tables Identity

| Table | Description |
|-------|-------------|
| `AspNetUsers` | Utilisateurs de l'application |
| `AspNetRoles` | Rôles (Admin, Manager, User) |
| `AspNetUserRoles` | Association utilisateur-rôle |

## 🔍 Vues Utilitaires

- **`VehiclesWithCurrentDriver`** : Véhicules avec leur conducteur actuel
- **`MaintenanceDue`** : Véhicules nécessitant une maintenance

## 🎯 Enums et Types

```csharp
// Types de carburant
FuelType: Gasoline=0, Diesel=1, Electric=2, Hybrid=3

// Statuts véhicule
VehicleStatus: Inactive=0, Active=1, Maintenance=2, Decommissioned=3

// Types de permis
LicenseType: B=0, C=1, D=2, BE=3, CE=4, DE=5

// Statuts conducteur
DriverStatus: Inactive=0, Active=1, Suspended=2
```

## 🛠️ Dépannage

### Erreurs Communes

1. **Erreur de connexion PostgreSQL**
   ```bash
   # Vérifier que PostgreSQL est démarré
   systemctl status postgresql
   # ou
   net start postgresql-x64-13
   ```

2. **Outils EF Core manquants**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. **Conflits de versions de packages**
   ```bash
   dotnet restore
   dotnet clean
   dotnet build
   ```

4. **Permissions PostgreSQL**
   ```sql
   GRANT ALL PRIVILEGES ON DATABASE fleetsync_dev TO fleetsync_user;
   GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO fleetsync_user;
   ```

### Logs et Débogage

```bash
# Verbose EF Core
dotnet ef database update --verbose

# Logs PostgreSQL
tail -f /var/log/postgresql/postgresql-13-main.log
```

## 📈 Performance

### Index Recommandés
- `IX_Vehicles_Status` : Filtrage par statut
- `IX_VehicleAssignments_IsActive` : Assignations actives
- `IX_GpsTrackingRecords_RecordedAt` : Suivi chronologique
- `IX_MaintenanceRecords_NextMaintenanceDue` : Maintenances dues

### Optimisations
- Partitioning pour `GpsTrackingRecords` (gros volume)
- Index composites pour les requêtes fréquentes
- Archivage des données historiques

## 🔒 Sécurité

- Mots de passe PostgreSQL forts
- Utilisateurs dédiés par environnement
- Chiffrement en transit (SSL)
- Backups réguliers
- Audit des accès

## 📚 Ressources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)

---

**FleetSyncManager Database Scripts** - Version 1.0.0 - 2025-08-28
