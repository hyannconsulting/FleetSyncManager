# FleetSyncManager - Système de Gestion de Flotte

## 🚗 Description

FleetSyncManager est un système complet de gestion de flotte automobile développé en .NET 9.0 avec une architecture Clean Architecture, utilisant Blazor Server pour l'interface utilisateur et des APIs REST avec Minimal API.

## 🏗️ Architecture

Le projet suit les principes de **Clean Architecture** avec 4 couches distinctes :

- **Domain** - Entités métier, règles business et interfaces
- **Application** - Services applicatifs, CQRS avec MediatR, DTOs
- **Infrastructure** - Accès aux données, services externes
- **Web** - Interface Blazor Server et API REST

## 🛠️ Technologies

- **.NET 9.0** - Framework principal
- **ASP.NET Core 9.0** - Web framework  
- **Blazor Server** - Interface utilisateur interactive
- **MediatR** - Pattern Mediator pour CQRS
- **FluentValidation** - Validation robuste
- **Entity Framework Core** - ORM pour PostgreSQL
- **AutoMapper** - Mapping objets
- **Serilog** - Logging structuré
- **Bootstrap 5.3** - Framework CSS responsive
- **Font Awesome** - Iconographie

## 🎯 Fonctionnalités

### Modules Principaux
1. **Gestion des Véhicules** - CRUD complet avec suivi kilométrage
2. **Gestion des Conducteurs** - Profils, licences, assignations
3. **Maintenance Préventive** - Planification et suivi des interventions
4. **Gestion d'Incidents** - Déclaration et traitement des sinistres
5. **Suivi GPS** - Géolocalisation temps réel des véhicules
6. **Tableaux de Bord** - Analytics et rapports personnalisés
7. **Administration** - Gestion utilisateurs et paramétrage
8. **API REST** - Intégration avec systèmes tiers

### Fonctionnalités Avancées
- **Notifications automatiques** (maintenance due, assurance expirée)
- **Géofencing** et alertes de zones
- **Rapports personnalisables** PDF/Excel
- **Interface responsive** mobile-friendly
- **Authentification sécurisée** JWT
- **Audit trail** complet

## 🚀 Installation

### Prérequis
- .NET 9.0 SDK
- PostgreSQL 15+
- Node.js (pour les assets frontend)
- Git

### Configuration
1. Cloner le repository
2. Configurer la base de données dans `appsettings.json`
3. Exécuter les migrations Entity Framework
4. Lancer l'application

```bash
git clone <repository-url>
cd Laroche.FleetManager.vibecoding
dotnet restore
dotnet ef database update --project src/Laroche.FleetManager.Infrastructure --startup-project src/Laroche.FleetManager.Web
dotnet run --project src/Laroche.FleetManager.Web
```

## 📁 Structure du Projet

```
src/
├── Laroche.FleetManager.Domain/          # Couche Domaine
├── Laroche.FleetManager.Application/     # Couche Application  
├── Laroche.FleetManager.Infrastructure/  # Couche Infrastructure
└── Laroche.FleetManager.Web/            # Couche Présentation
```

## 🔧 Configuration

### Base de Données
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fleetmanager;Username=postgres;Password=your_password"
  }
}
```

### Authentification
Configuration JWT dans `appsettings.json` pour sécuriser les API.

## 📱 API REST

L'application expose des API REST complètes :

- `GET /api/v1/vehicles` - Liste des véhicules
- `POST /api/v1/vehicles` - Créer un véhicule  
- `PUT /api/v1/vehicles/{id}` - Modifier un véhicule
- `DELETE /api/v1/vehicles/{id}` - Supprimer un véhicule

Documentation Swagger disponible sur `/api-docs`

## 🧪 Tests

```bash
dotnet test
```

## 📊 Monitoring

- **Logs structurés** avec Serilog
- **Health checks** pour surveillance
- **Métriques** de performance

## 🔒 Sécurité

- Authentification JWT
- Autorisation basée sur les rôles
- Validation côté serveur
- Protection CSRF
- HTTPS obligatoire en production

## 🌍 Déploiement

Support Docker et Azure pour déploiement cloud :

```dockerfile
# Dockerfile inclus pour containerisation
docker build -t fleetsynmanager .
docker run -p 5000:80 fleetsynmanager
```

## 👥 Contribution

1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/nouvelle-fonctionnalite`)
3. Commit les modifications (`git commit -am 'Ajout nouvelle fonctionnalité'`)
4. Push vers la branche (`git push origin feature/nouvelle-fonctionnalite`)
5. Créer une Pull Request

## 📄 Licence

Ce projet est sous licence MIT.

## 🎯 Roadmap

- [ ] Module de facturation
- [ ] Application mobile native
- [ ] Intégration IoT capteurs véhicules
- [ ] IA pour maintenance prédictive
- [ ] API GraphQL
- [ ] PWA offline-first

## 📞 Support

Pour questions et support : support@fleetsynmanager.com

---
**FleetSyncManager** - *La solution complète pour votre gestion de flotte*