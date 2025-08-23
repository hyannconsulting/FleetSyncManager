# User Stories et Spécifications Techniques MVP

## User Stories par Rôle

### 👨‍💼 ADMINISTRATEUR

#### Epic 1: Gestion du Système et des Utilisateurs

**US-001: Authentification Sécurisée**
```
En tant qu'Administrateur,
Je veux me connecter de manière sécurisée au système
Afin d'accéder aux fonctionnalités d'administration avec contrôle des accès.

Critères d'Acceptation:
✅ Connexion avec email/mot de passe
✅ Session expirante après inactivité (30 min)
✅ Protection contre attaques par force brute
✅ Logs des connexions avec IP et timestamp
✅ Possibilité de réinitialiser mot de passe

Complexité: Small (3 points)
```

**US-002: Gestion des Utilisateurs**
```
En tant qu'Administrateur,
Je veux créer, modifier et désactiver les comptes utilisateurs
Afin de contrôler l'accès au système selon les rôles.

Critères d'Acceptation:
✅ CRUD complet des comptes utilisateurs
✅ Attribution des rôles (Admin, Gestionnaire, Conducteur)
✅ Activation/désactivation des comptes
✅ Envoi automatique d'email avec identifiants
✅ Interface de recherche et filtrage des utilisateurs

Complexité: Medium (5 points)
```

#### Epic 2: Configuration Système

**US-003: Paramétrage des Alertes**
```
En tant qu'Administrateur,
Je veux configurer les seuils et fréquences des alertes
Afin d'adapter le système aux besoins de l'organisation.

Critères d'Acceptation:
✅ Configuration des délais d'alerte (CT, assurance, révision)
✅ Paramétrage des destinataires par type d'alerte
✅ Templates d'emails personnalisables
✅ Activation/désactivation par type d'alerte
✅ Prévisualisation des emails

Complexité: Medium (5 points)
```

### 👨‍💻 GESTIONNAIRE DE FLOTTE

#### Epic 3: Gestion des Véhicules

**US-004: Inventaire des Véhicules**
```
En tant que Gestionnaire,
Je veux consulter la liste complète des véhicules
Afin d'avoir une vue d'ensemble de la flotte.

Critères d'Acceptation:
✅ Liste paginée avec 50 véhicules par page
✅ Colonnes: Immatriculation, Marque/Modèle, Conducteur, Statut, Prochaine échéance
✅ Filtres: Statut, Marque, Type, Conducteur affecté
✅ Recherche globale sur immatriculation et modèle
✅ Tri sur toutes les colonnes
✅ Export Excel de la liste filtrée

Complexité: Medium (5 points)
```

**US-005: Fiche Véhicule Complète**
```
En tant que Gestionnaire,
Je veux accéder aux informations détaillées d'un véhicule
Afin de gérer efficacement chaque véhicule de la flotte.

Critères d'Acceptation:
✅ Onglet Infos générales: caractéristiques, photo, dates importantes
✅ Onglet Documents: upload/download carte grise, assurance, contrats
✅ Onglet Affectations: historique et conducteur actuel
✅ Onglet Maintenance: historique et planification
✅ Alertes visuelles pour échéances proches (<30 jours)
✅ Modification des informations avec validation

Complexité: Large (8 points)
```

**US-006: Ajout de Nouveau Véhicule**
```
En tant que Gestionnaire,
Je veux ajouter un nouveau véhicule à la flotte
Afin d'enrichir l'inventaire avec toutes les informations nécessaires.

Critères d'Acceptation:
✅ Formulaire multi-étapes: Identification → Caractéristiques → Acquisition → Assurance
✅ Validation des champs obligatoires
✅ Vérification unicité immatriculation
✅ Upload photo et documents principaux
✅ Configuration automatique des alertes selon les dates
✅ Notification de création envoyée

Complexité: Medium (5 points)
```

#### Epic 4: Gestion des Conducteurs

**US-007: Répertoire des Conducteurs**
```
En tant que Gestionnaire,
Je veux consulter la liste de tous les conducteurs
Afin de gérer les affectations et suivre leur statut.

Critères d'Acceptation:
✅ Liste avec photo, nom, véhicule affecté, statut permis
✅ Filtres: Statut permis, Véhicule affecté, Département
✅ Recherche par nom/prénom
✅ Indicateurs visuels pour échéances proches
✅ Actions rapides: voir profil, affecter véhicule

Complexité: Medium (5 points)
```

**US-008: Profil Conducteur Détaillé**
```
En tant que Gestionnaire,
Je veux accéder au profil complet d'un conducteur
Afin de gérer ses informations et son historique.

Critères d'Acceptation:
✅ Onglet Personnel: coordonnées, photo, contact urgence
✅ Onglet Permis: types, dates expiration, restrictions
✅ Onglet Véhicules: historique affectations
✅ Onglet Infractions: contraventions, points restants
✅ Upload documents (permis, visite médicale)
✅ Modification avec traçabilité des changements

Complexité: Large (8 points)
```

#### Epic 5: Suivi et Alertes

**US-009: Tableau de Bord Opérationnel**
```
En tant que Gestionnaire,
Je veux avoir une vue d'ensemble des indicateurs clés
Afin de piloter efficacement la flotte au quotidien.

Critères d'Acceptation:
✅ KPI: Nb véhicules, conducteurs, sinistres actifs, coût mensuel
✅ Widget alertes urgentes avec compteur et liste
✅ Graphique évolution kilométrage mensuel
✅ Liste véhicules en maintenance
✅ Derniers sinistres déclarés
✅ Accès rapide aux actions critiques

Complexité: Large (8 points)
```

**US-010: Système d'Alertes Email**
```
En tant que Gestionnaire,
Je veux recevoir des alertes automatiques par email
Afin d'être notifié des échéances importantes sans oubli.

Critères d'Acceptation:
✅ Alerte CT à 60, 30 et 15 jours de l'échéance
✅ Alerte assurance à 30 et 15 jours
✅ Alerte révision selon kilométrage ou délai
✅ Email avec détails: véhicule, conducteur, type échéance, date
✅ Liens directs vers les fiches concernées
✅ Possibilité de reporter une alerte

Complexité: Medium (5 points)
```

#### Epic 6: Gestion des Sinistres

**US-011: Déclaration de Sinistre**
```
En tant que Gestionnaire,
Je veux enregistrer un nouveau sinistre
Afin de centraliser et tracer tous les incidents.

Critères d'Acceptation:
✅ Formulaire guidé: Identification → Circonstances → Dégâts → Tiers → Témoins
✅ Sélection véhicule et conducteur avec auto-complétion
✅ Upload photos et documents (constat, devis)
✅ Géolocalisation du lieu si disponible
✅ Génération automatique numéro de dossier
✅ Notification email aux parties concernées

Complexité: Large (8 points)
```

**US-012: Suivi des Dossiers Sinistres**
```
En tant que Gestionnaire,
Je veux suivre l'avancement des dossiers sinistres
Afin de m'assurer de leur bonne résolution.

Critères d'Acceptation:
✅ Dashboard avec statuts: Déclaré, En cours, Expertise, Réparation, Clos
✅ Workflow avec transitions et historique
✅ Communication avec assureurs (emails, courriers)
✅ Suivi des coûts (franchise, réparations)
✅ Indicateurs de performance (délai moyen, coût)

Complexité: Large (8 points)
```

### 🚗 CONDUCTEUR

#### Epic 7: Interface Conducteur

**US-013: Consultation Véhicule Affecté**
```
En tant que Conducteur,
Je veux consulter les informations de mon véhicule affecté
Afin de connaître son état et les échéances importantes.

Critères d'Acceptation:
✅ Vue simplifiée: photo, caractéristiques principales
✅ Prochaines échéances avec alertes visuelles
✅ Kilométrage actuel et prochain entretien
✅ Documents accessibles (assurance, carte grise)
✅ Bouton d'accès rapide pour mise à jour km

Complexité: Small (3 points)
```

**US-014: Mise à Jour Kilométrage**
```
En tant que Conducteur,
Je veux mettre à jour le kilométrage de mon véhicule
Afin de maintenir les données à jour pour la maintenance.

Critères d'Acceptation:
✅ Formulaire simple avec validation
✅ Photo du compteur obligatoire
✅ Vérification cohérence (pas de retour en arrière)
✅ Historique des saisies précédentes
✅ Notification gestionnaire si écart important

Complexité: Small (3 points)
```

**US-015: Déclaration Rapide Incident**
```
En tant que Conducteur,
Je veux déclarer rapidement un incident
Afin d'alerter immédiatement les gestionnaires.

Critères d'Acceptation:
✅ Formulaire simplifié mobile-friendly
✅ Photo obligatoire des dégâts
✅ Géolocalisation automatique
✅ Description vocale convertie en texte
✅ Envoi immédiat avec accusé réception
✅ Suivi du statut de traitement

Complexité: Medium (5 points)
```

## Spécifications Techniques par User Story

### Architecture Technique

#### Structure du Projet .NET
```
FleetSyncManager/
├── src/
│   ├── FleetSync.Web/              # Application Web (Blazor Server)
│   ├── FleetSync.API/              # API REST pour mobile
│   ├── FleetSync.Core/             # Business Logic & Entities
│   ├── FleetSync.Infrastructure/   # Data Access & External Services
│   └── FleetSync.Shared/           # Shared Models & DTOs
├── tests/
│   ├── FleetSync.UnitTests/
│   ├── FleetSync.IntegrationTests/
│   └── FleetSync.E2ETests/
└── database/
    ├── migrations/
    └── seed-data/
```

#### Stack Technique
- **Backend:** ASP.NET Core 8.0, Entity Framework Core
- **Frontend:** Blazor Server avec Bootstrap 5
- **Database:** PostgreSQL 15+
- **Authentication:** ASP.NET Core Identity
- **File Storage:** Local Storage + Azure Blob (future)
- **Email:** SMTP + SendGrid (future)
- **Documentation:** OpenAPI/Swagger

### Modèles de Données Principaux

#### Entité Vehicle
```csharp
public class Vehicle
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } // Immatriculation
    public string Vin { get; set; } // Numéro de châssis
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int CurrentMileage { get; set; }
    public FuelType FuelType { get; set; }
    public VehicleStatus Status { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public DateTime NextTechnicalInspection { get; set; }
    public DateTime InsuranceExpiryDate { get; set; }
    
    // Relations
    public int? AssignedDriverId { get; set; }
    public Driver? AssignedDriver { get; set; }
    public ICollection<VehicleDocument> Documents { get; set; }
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
    public ICollection<Incident> Incidents { get; set; }
}
```

#### Entité Driver
```csharp
public class Driver
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime LicenseObtainedDate { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
    public string LicenseTypes { get; set; } // JSON array
    public int RemainingPoints { get; set; }
    public DriverStatus Status { get; set; }
    
    // Relations
    public ICollection<Vehicle> AssignedVehicles { get; set; }
    public ICollection<DriverDocument> Documents { get; set; }
    public ICollection<Violation> Violations { get; set; }
}
```

### APIs RESTful

#### Endpoints Véhicules
```http
GET    /api/vehicles              # Liste paginée avec filtres
GET    /api/vehicles/{id}         # Détail véhicule
POST   /api/vehicles              # Création véhicule  
PUT    /api/vehicles/{id}         # Modification
DELETE /api/vehicles/{id}         # Suppression
GET    /api/vehicles/{id}/documents # Documents du véhicule
POST   /api/vehicles/{id}/documents # Upload document
```

#### Endpoints Conducteurs
```http
GET    /api/drivers               # Liste paginée
GET    /api/drivers/{id}          # Profil conducteur
POST   /api/drivers               # Création
PUT    /api/drivers/{id}          # Modification
POST   /api/drivers/{id}/assign-vehicle/{vehicleId} # Affectation
```

#### Endpoints Sinistres
```http
GET    /api/incidents             # Liste des sinistres
GET    /api/incidents/{id}        # Détail sinistre
POST   /api/incidents             # Déclaration
PUT    /api/incidents/{id}/status # Changement statut
POST   /api/incidents/{id}/documents # Upload documents
```

### Services et Intégrations

#### Service d'Alertes
```csharp
public interface IAlertService
{
    Task CheckTechnicalInspectionAlerts();
    Task CheckInsuranceExpiryAlerts();
    Task CheckMaintenanceAlerts();
    Task SendAlert(AlertType type, Vehicle vehicle, int daysRemaining);
}
```

#### Service de Notifications
```csharp
public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSMSAsync(string phoneNumber, string message);
    Task<bool> IsEmailConfigured();
}
```

### Sécurité et Permissions

#### Rôles et Autorisations
```csharp
public static class Roles
{
    public const string Administrator = "Administrator";
    public const string FleetManager = "FleetManager";
    public const string Driver = "Driver";
}

public static class Permissions
{
    public const string ManageUsers = "manage.users";
    public const string ViewAllVehicles = "view.all.vehicles";
    public const string ManageVehicles = "manage.vehicles";
    public const string ViewOwnVehicle = "view.own.vehicle";
    public const string DeclareIncident = "declare.incident";
}
```

### Tests et Qualité

#### Tests Unitaires Critiques
- Validation des modèles de données
- Logique métier (calculs, règles)
- Services d'authentification
- Calculs d'alertes et notifications

#### Tests d'Intégration
- API endpoints avec base de données
- Service d'email
- Upload/download de fichiers
- Workflows de sinistres

Cette spécification technique détaillée guide l'implémentation de chaque user story avec les patterns et technologies appropriés pour un MVP robuste et évolutif.
