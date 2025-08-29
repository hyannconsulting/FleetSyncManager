# Configuration Base de Données PostgreSQL - FleetSyncManager

## 🗃️ Vue d'ensemble

FleetSyncManager utilise PostgreSQL comme base de données principale avec une option de base de données en mémoire pour les tests. La configuration supporte les environnements de développement, test et production.

## 🚀 Démarrage Rapide

### 1. Démarrer PostgreSQL avec Docker

```bash
# Démarrer PostgreSQL et PgAdmin
.\database.ps1 start

# Ou utilisez Docker Compose directement
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Appliquer les Migrations

```bash
cd src\Laroche.FleetManager.Web
dotnet ef database update --project ..\Laroche.FleetManager.Infrastructure
```

### 3. Lancer l'Application

```bash
dotnet run
```

## 🔧 Configuration

### Chaînes de Connexion

**Développement (PostgreSQL local):**
```json
"DefaultConnection": "Host=localhost;Port=5432;Database=fleetmanager_dev;Username=fleetmanager;Password=DevPassword123!;Pooling=true;MinPoolSize=1;MaxPoolSize=20;"
```

**Tests (Base de données en mémoire):**
```json
"InMemoryConnection": "InMemoryDatabase"
```

### Paramètres de Base de Données

```json
{
  "DatabaseSettings": {
    "UseInMemoryDatabase": false,    // true pour utiliser la base en mémoire
    "AutoMigrate": true,             // Migrations automatiques au démarrage
    "SeedSampleData": true,          // Données d'exemple en développement
    "ConnectionTimeout": 30,         // Timeout de connexion (secondes)
    "CommandTimeout": 60,            // Timeout des commandes (secondes)
    "MaxRetryCount": 3,              // Nombre de tentatives de reconnexion
    "MaxRetryDelay": "00:00:30"      // Délai maximum entre les tentatives
  }
}
```

## 🐳 Services Docker

### PostgreSQL
- **Port:** 5432
- **Base:** `fleetmanager_dev`
- **Utilisateur:** `fleetmanager`
- **Mot de passe:** `DevPassword123!`

### PgAdmin (Interface d'administration)
- **URL:** http://localhost:5050
- **Email:** admin@fleetmanager.local
- **Mot de passe:** AdminPassword123!

### Redis (Cache - Optionnel)
- **Port:** 6379
- **Mot de passe:** RedisPassword123!

## 📜 Scripts de Gestion

### Script PowerShell `database.ps1`

```bash
# Démarrer les services
.\database.ps1 start

# Arrêter les services
.\database.ps1 stop

# Redémarrer les services
.\database.ps1 restart

# Réinitialiser la base de données
.\database.ps1 reset

# Voir les logs
.\database.ps1 logs

# Statut des services
.\database.ps1 status

# Sauvegarder la base
.\database.ps1 backup

# Aide
.\database.ps1 help
```

## 🔄 Migrations Entity Framework

### Créer une Migration

```bash
cd src\Laroche.FleetManager.Infrastructure
dotnet ef migrations add NomDeLaMigration --startup-project ..\Laroche.FleetManager.Web
```

### Appliquer les Migrations

```bash
cd src\Laroche.FleetManager.Web
dotnet ef database update --project ..\Laroche.FleetManager.Infrastructure
```

### Supprimer la Dernière Migration

```bash
cd src\Laroche.FleetManager.Infrastructure
dotnet ef migrations remove --startup-project ..\Laroche.FleetManager.Web
```

## 🧪 Tests avec Base de Données en Mémoire

Pour utiliser la base de données en mémoire (utile pour les tests) :

```json
{
  "DatabaseSettings": {
    "UseInMemoryDatabase": true
  }
}
```

Ou par variable d'environnement :
```bash
export ASPNETCORE_ENVIRONMENT=Testing
```

## 🏗️ Architecture

```
📁 Infrastructure/
├── 📁 Configuration/
│   └── DatabaseSettings.cs        # Configuration base de données
├── 📁 Data/
│   ├── ApplicationDbContext.cs     # Contexte EF Core principal
│   └── 📁 Configurations/          # Configuration des entités
├── 📁 Extensions/
│   └── DatabaseServiceExtensions.cs # Extensions de services
└── 📁 Repositories/                # Implémentations repositories
```

## 🔒 Sécurité

### Développement
- Mots de passe par défaut (à changer en production)
- Logs détaillés activés
- Données sensibles visibles dans les logs

### Production
- Variables d'environnement pour les mots de passe
- SSL/TLS obligatoire
- Logs sécurisés
- Rotation des mots de passe

## 📊 Monitoring

### Vérification de Santé

L'application inclut des health checks pour :
- Connexion à la base de données
- État des migrations
- Performances des requêtes

Accès via : `https://localhost:7001/health`

### Logs

Les logs sont configurés avec Serilog :
- Console (développement)
- Fichiers rotatifs (production)
- Structured logging pour monitoring

## 🔧 Dépannage

### Problèmes Courants

**1. Connexion refusée**
```bash
# Vérifier que PostgreSQL est démarré
.\database.ps1 status

# Redémarrer les services
.\database.ps1 restart
```

**2. Migration échoue**
```bash
# Vérifier les logs PostgreSQL
.\database.ps1 logs

# Réinitialiser la base
.\database.ps1 reset
```

**3. Données corrompues**
```bash
# Sauvegarder d'abord
.\database.ps1 backup

# Puis réinitialiser
.\database.ps1 reset
```

### Variables d'Environnement

```bash
# Forcer l'utilisation de la base en mémoire
ASPNETCORE_DatabaseSettings__UseInMemoryDatabase=true

# Désactiver les migrations automatiques
ASPNETCORE_DatabaseSettings__AutoMigrate=false

# Désactiver les données d'exemple
ASPNETCORE_DatabaseSettings__SeedSampleData=false
```

## 📚 Ressources

- [Documentation PostgreSQL](https://www.postgresql.org/docs/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Npgsql Provider](https://www.npgsql.org/efcore/)
- [Docker Compose](https://docs.docker.com/compose/)

---
**FleetSyncManager** - Configuration PostgreSQL v1.0
