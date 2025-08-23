# FleetSyncManager - Clean Architecture Solution

## Architecture Overview

Cette solution suit les principes de Clean Architecture avec .NET 9.0, MediatR, et Blazor Server.

### Structure des Projets

```
src/
├── Laroche.FleetManager.Domain/          # Couche Domaine
│   ├── Entities/                         # Entités métier
│   ├── Enums/                           # Énumérations
│   ├── Interfaces/                       # Interfaces du domaine
│   ├── ValueObjects/                     # Objets valeur
│   ├── Events/                          # Événements du domaine
│   └── Common/                          # Classes de base
│
├── Laroche.FleetManager.Application/     # Couche Application
│   ├── Commands/                        # Commands (CQRS)
│   ├── Queries/                         # Queries (CQRS)
│   ├── DTOs/                           # Data Transfer Objects
│   ├── Services/                        # Services application
│   ├── Interfaces/                      # Interfaces application
│   ├── Behaviors/                       # Behaviors MediatR
│   ├── Validators/                      # Validateurs FluentValidation
│   └── Common/                          # Classes communes
│
├── Laroche.FleetManager.Infrastructure/  # Couche Infrastructure
│   ├── Data/                           # Contexte Entity Framework
│   ├── Repositories/                    # Implémentations repositories
│   ├── Services/                        # Services externes
│   ├── Configuration/                   # Configuration EF
│   └── Migrations/                      # Migrations EF
│
└── Laroche.FleetManager.Web/            # Couche Présentation
    ├── Components/                      # Composants Blazor
    │   ├── Layout/                     # Layout components
    │   └── Pages/                      # Pages Blazor
    ├── Controllers/                     # Contrôleurs API (optionnel)
    ├── Services/                        # Services UI
    └── wwwroot/                        # Ressources statiques
```

## Technologies Utilisées

- **.NET 9.0** - Framework principal
- **ASP.NET Core 9.0** - Web framework
- **Blazor Server** - Interface utilisateur
- **MediatR** - Pattern Mediator/CQRS
- **FluentValidation** - Validation des commandes
- **AutoMapper** - Mapping objets
- **Entity Framework Core** - ORM
- **PostgreSQL** - Base de données
- **Bootstrap 5.3** - Framework CSS
- **Font Awesome** - Icônes
- **Serilog** - Logging

## Fonctionnalités Principales

### Modules Métier
1. **Gestion des Véhicules** - CRUD véhicules avec suivi
2. **Gestion des Conducteurs** - Profils et assignations
3. **Maintenance** - Planification et suivi
4. **Incidents** - Déclaration et gestion
5. **Suivi GPS** - Géolocalisation temps réel
6. **Rapports** - Tableaux de bord et analytics

### API Endpoints (Minimal API)
```
GET    /api/v1/vehicles              - Liste des véhicules
GET    /api/v1/vehicles/{id}         - Détails véhicule
POST   /api/v1/vehicles              - Créer véhicule
PUT    /api/v1/vehicles/{id}         - Modifier véhicule
DELETE /api/v1/vehicles/{id}         - Supprimer véhicule
GET    /api/v1/vehicles/driver/{id}  - Véhicules par conducteur
```

## Configuration

### Base de Données
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fleetmanager;Username=postgres;Password=your_password"
  }
}
```

### Logging (Serilog)
```json
{
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/fleetmanager-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Prochaines Étapes

1. **Configuration Entity Framework** avec PostgreSQL
2. **Implémentation des Handlers** MediatR
3. **Configuration de l'authentification** JWT/Identity
4. **Tests unitaires** et d'intégration
5. **Docker** containerisation
6. **CI/CD** pipeline

## Commandes Utiles

### Build Solution
```bash
dotnet build
```

### Run Application
```bash
dotnet run --project src/Laroche.FleetManager.Web
```

### Add Migration (quand EF sera configuré)
```bash
dotnet ef migrations add InitialCreate --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
```

### Update Database
```bash
dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
```

## Standards de Développement

- **Clean Architecture** - Séparation des responsabilités
- **CQRS** - Séparation lecture/écriture
- **Repository Pattern** - Abstraction données
- **Unit of Work** - Gestion transactions
- **Domain Events** - Communication découplée
- **Validation** - FluentValidation sur commandes
- **Logging** - Serilog structuré
- **Exception Handling** - Gestion centralisée

Cette structure fournit une base solide et scalable pour FleetSyncManager.
