# 🏗️ Refactoring Architecture - API Séparée

## ✅ Modifications Apportées

### 🆕 Nouveau Projet : Laroche.FleetManager.API

**Rôle :** API REST pure avec contrôleurs minimalistes, dédiée aux services backend.

**Technologie :**
- .NET 9.0
- Minimal API avec Swagger
- MediatR pour CQRS
- Serilog pour les logs
- JWT pour l'authentification
- Health Checks

**Structure :**
```
src/Laroche.FleetManager.API/
├── Program.cs                      # Point d'entrée API
├── appsettings.json               # Configuration API
├── Extensions/
│   ├── ServiceCollectionExtensions.cs  # Services API
│   └── WebApplicationExtensions.cs     # Configuration endpoints
├── Endpoints/
│   ├── VehicleEndpoints.cs        # Endpoints véhicules
│   ├── DriverEndpoints.cs         # Endpoints conducteurs
│   ├── MaintenanceEndpoints.cs    # Endpoints maintenance
│   └── IncidentEndpoints.cs       # Endpoints incidents
└── Middleware/
    └── ExceptionHandlingMiddleware.cs  # Gestion erreurs
```

### 🔄 Projet Modifié : Laroche.FleetManager.Web

**Nouveau Rôle :** Interface utilisateur Blazor Server qui consomme l'API REST.

**Changements :**
- ✅ Suppression des endpoints API (déplacés vers Laroche.FleetManager.API)
- ✅ Ajout des services HttpClient pour consommer l'API
- ✅ Configuration simplifiée (uniquement Blazor Server + clients HTTP)
- ✅ Suppression des dépendances MediatR, AutoMapper, JWT (gérés par l'API)

**Nouvelle Structure :**
```
src/Laroche.FleetManager.Web/
├── Program.cs                     # Configuration Blazor + HttpClient
├── appsettings.json              # Config client (URL API)
├── Services/                     # Services clients API
│   ├── BaseApiClientService.cs   # Service de base HTTP
│   ├── VehicleApiService.cs      # Client API véhicules
│   ├── DriverApiService.cs       # Client API conducteurs
│   ├── MaintenanceApiService.cs  # Client API maintenance
│   └── IncidentApiService.cs     # Client API incidents
├── Components/                   # Composants Blazor (existants)
├── Pages/                       # Pages Blazor (existantes)
└── wwwroot/                     # Assets statiques (existants)
```

## 🎯 Avantages de cette Architecture

### 🔀 Séparation des Préoccupations
- **API** : Se concentre uniquement sur les services métier et la logique backend
- **Web** : Se concentre uniquement sur l'interface utilisateur et l'expérience client

### 📈 Scalabilité
- L'API peut être déployée indépendamment sur plusieurs instances
- Le client Web peut être déployé sur des serveurs dédiés à l'interface
- Possibilité d'avoir plusieurs clients (Web, Mobile, Desktop) consommant la même API

### 🛠️ Maintenance
- **API** : Évolutions métier et performance backend
- **Web** : Évolutions UX/UI et fonctionnalités client
- Tests séparés pour chaque couche

### 🔐 Sécurité
- L'API peut implémenter l'authentification JWT
- Le client Web n'expose pas directement la logique métier
- CORS configuré spécifiquement pour les clients autorisés

## 🚀 Configuration de Développement

### Ports par Défaut
- **API** : https://localhost:7002 (swagger à la racine)
- **Web** : https://localhost:7001 (interface Blazor)

### Démarrage des Applications

#### 1. Démarrer l'API
```bash
cd src/Laroche.FleetManager.API
dotnet run
```
Swagger disponible sur : https://localhost:7002

#### 2. Démarrer le Client Web
```bash
cd src/Laroche.FleetManager.Web
dotnet run
```
Interface disponible sur : https://localhost:7001

### Configuration des URLs

**Dans appsettings.json de l'API :**
```json
{
  "CORS": {
    "BlazorClientUrls": [
      "https://localhost:7001",
      "http://localhost:5001"
    ]
  }
}
```

**Dans appsettings.json du Web :**
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7002"
  }
}
```

## 🔧 Services HTTP Client

### Pattern Utilisé
Tous les services héritent de `BaseApiClientService<TDto, TCreateCommand, TUpdateCommand>` qui fournit :
- **GetAllAsync** : Récupération avec pagination
- **GetByIdAsync** : Récupération par ID
- **CreateAsync** : Création d'entité
- **UpdateAsync** : Mise à jour d'entité
- **DeleteAsync** : Suppression d'entité

### Services Spécialisés
- **VehicleApiService** : Gestion véhicules + méthodes spécifiques
- **DriverApiService** : Gestion conducteurs
- **MaintenanceApiService** : Gestion maintenance + requêtes spécialisées
- **IncidentApiService** : Gestion incidents + filtres spéciaux

## 📋 Prochaines Étapes

### Phase 1 - Validation Architecture ✅
- [x] Création projet API avec endpoints Minimal API
- [x] Refactoring projet Web en client HTTP
- [x] Configuration CORS et communication inter-services
- [ ] **Tests de compilation et démarrage**

### Phase 2 - Configuration Base de Données
- [ ] Configuration Entity Framework dans l'Infrastructure
- [ ] Migrations PostgreSQL
- [ ] Tests d'intégration API + Base de données

### Phase 3 - Tests et Intégration
- [ ] Tests unitaires pour les services HTTP client
- [ ] Tests d'intégration API
- [ ] Validation complète du workflow

### Phase 4 - Déploiement
- [ ] Configuration Docker pour API et Web
- [ ] Scripts de déploiement séparés
- [ ] Configuration production (variables d'environnement)

## 🎉 Résultat Final

Cette refactorisation respecte les **meilleures pratiques architecturales** :
- ✅ **Single Responsibility** : Chaque projet a un rôle clair
- ✅ **Loose Coupling** : Communication via HTTP REST
- ✅ **High Cohesion** : Logique métier centralisée dans l'API
- ✅ **Testability** : Tests séparés par couche
- ✅ **Scalability** : Déploiement indépendant possible

**L'architecture est maintenant prête pour le développement des 4 sprints MVP avec une base solide et évolutive !** 🚀
