# GitHub Issues - Backlog MVP FleetSyncManager

Ce document contient les spécifications détaillées des issues GitHub pour le développement du MVP de FleetSyncManager. Chaque issue suit le template du Product Manager Assistant avec une approche MVP-first.

## Issues Critiques - Version 1.0

### Issue #1: [CRITICAL] Système d'Authentification et Gestion des Rôles

**Labels:** `enhancement`, `critical`, `mvp-core`, `security`

## Overview
Implémentation du système d'authentification sécurisé avec gestion des rôles (Admin, Gestionnaire, Conducteur) pour contrôler l'accès aux fonctionnalités selon les permissions utilisateur.

## Scope
### Inclus
- Connexion/déconnexion avec email/mot de passe
- Gestion des sessions avec expiration automatique
- Trois rôles principaux avec permissions différenciées
- Protection contre attaques par force brute
- Interface de récupération de mot de passe
- Logs des connexions avec audit trail

### Exclus (versions futures)
- Authentification multi-facteurs (MFA)
- Single Sign-On (SSO)
- Authentification sociale (Google, Microsoft)
- API tokens pour intégrations externes

## Technical Requirements
- **Framework:** ASP.NET Core Identity avec Entity Framework
- **Base de données:** Tables Users, Roles, UserRoles dans PostgreSQL
- **Sécurité:** Hash des mots de passe avec BCrypt, sessions sécurisées
- **UI:** Pages Blazor pour connexion/déconnexion
- **Validation:** Côté client et serveur avec Data Annotations

## Implementation Plan

### Étape 1: Configuration ASP.NET Core Identity
```csharp
// Startup.cs
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
```

### Étape 2: Modèles de données
```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public static class ApplicationRoles
{
    public const string Administrator = "Administrator";
    public const string FleetManager = "FleetManager";
    public const string Driver = "Driver";
}
```

### Étape 3: Pages d'authentification Blazor
- `/Pages/Account/Login.razor`
- `/Pages/Account/Logout.razor`
- `/Pages/Account/ForgotPassword.razor`
- `/Components/LoginDisplay.razor`

### Étape 4: Service d'audit des connexions
```csharp
public interface IAuditService
{
    Task LogLoginAttempt(string email, string ipAddress, bool success);
    Task LogUserAction(string userId, string action, string details);
}
```

## Acceptance Criteria
- [ ] Un utilisateur peut se connecter avec email/mot de passe valides
- [ ] Session expire automatiquement après 30 minutes d'inactivité
- [ ] Compte verrouillé après 5 tentatives échouées pendant 15 minutes
- [ ] Trois rôles créés automatiquement au démarrage
- [ ] Interface de récupération de mot de passe fonctionnelle
- [ ] Logs de connexion stockés avec IP et timestamp
- [ ] Tests unitaires pour l'authentification (couverture > 80%)
- [ ] Pages sécurisées inaccessibles sans connexion

## Priority
**Score de priorité:** 6.25 (Critical)
**Justification:** Fondation sécuritaire indispensable, bloque tous les autres développements

## Dependencies
- **Blocks:** Toutes les autres issues (#2, #3, #4, #5, #6)
- **Blocked by:** Aucune (issue fondamentale)

## Implementation Size
- **Estimated effort:** Small (3-5 jours)
- **Complexity:** Faible (technologies standards)

---

### Issue #2: [CRITICAL] Gestion Complète des Véhicules

**Labels:** `enhancement`, `critical`, `mvp-core`, `vehicles`

## Overview
Module complet de gestion des véhicules avec CRUD, gestion des documents, suivi des affectations et alertes automatiques pour les échéances importantes.

## Scope
### Inclus
- CRUD complet des véhicules avec validation
- Fiche détaillée multi-onglets (Infos, Documents, Affectations, Maintenance)
- Upload/gestion des documents (carte grise, assurance, contrats)
- Affectation aux conducteurs avec historique
- Calcul automatique des alertes (CT, assurance, révision)
- Interface de liste avec filtres et recherche
- Export Excel de la liste

### Exclus (versions futures)
- Géolocalisation GPS en temps réel
- Intégration avec constructeurs (API)
- Code QR/Barcode sur les véhicules
- Analyses prédictives de maintenance

## Technical Requirements
- **Entités:** Vehicle, VehicleDocument, VehicleAssignment
- **Storage:** Système de fichiers local pour documents (Azure Blob future)
- **Validation:** Règles métier pour immatriculations, dates cohérentes
- **UI:** Interface Blazor responsive avec upload de fichiers
- **API:** Endpoints RESTful pour utilisation mobile future

## Implementation Plan

### Étape 1: Modèles de données
```csharp
public class Vehicle
{
    public int Id { get; set; }
    [Required, StringLength(20)]
    public string LicensePlate { get; set; }
    public string Vin { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int CurrentMileage { get; set; }
    public FuelType FuelType { get; set; }
    public VehicleStatus Status { get; set; }
    public DateTime NextTechnicalInspection { get; set; }
    public DateTime InsuranceExpiryDate { get; set; }
    
    // Navigation properties
    public int? AssignedDriverId { get; set; }
    public Driver AssignedDriver { get; set; }
    public ICollection<VehicleDocument> Documents { get; set; }
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
}
```

### Étape 2: Repository Pattern
```csharp
public interface IVehicleRepository
{
    Task<PagedResult<Vehicle>> GetVehiclesAsync(VehicleFilter filter);
    Task<Vehicle> GetByIdAsync(int id);
    Task<Vehicle> CreateAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(int id);
    Task<bool> IsLicensePlateUniqueAsync(string licensePlate, int? excludeId = null);
}
```

### Étape 3: Service métier
```csharp
public class VehicleService : IVehicleService
{
    public async Task<Vehicle> CreateVehicleAsync(CreateVehicleDto dto)
    {
        // Validation métier
        await ValidateLicensePlate(dto.LicensePlate);
        
        var vehicle = _mapper.Map<Vehicle>(dto);
        
        // Configuration automatique des alertes
        SetupAutomaticAlerts(vehicle);
        
        return await _repository.CreateAsync(vehicle);
    }
}
```

### Étape 4: Interfaces Blazor
- `/Pages/Vehicles/VehiclesList.razor` - Liste avec filtres
- `/Pages/Vehicles/VehicleDetails.razor` - Fiche détaillée
- `/Pages/Vehicles/CreateVehicle.razor` - Formulaire création
- `/Components/VehicleDocuments.razor` - Gestion documents

## Acceptance Criteria
- [ ] Liste des véhicules avec pagination (50/page) et filtres fonctionnels
- [ ] Recherche par immatriculation et modèle opérationnelle
- [ ] Fiche véhicule avec 4 onglets complets (Infos, Documents, Affectations, Maintenance)
- [ ] Upload de documents avec validation des types (PDF, images)
- [ ] Validation unicité immatriculation avec message d'erreur clair
- [ ] Affectation de conducteur avec historique automatique
- [ ] Alertes visuelles pour échéances < 30 jours
- [ ] Export Excel avec données filtrées
- [ ] Tests unitaires pour validation et logique métier
- [ ] Interface responsive desktop/tablette

## Priority
**Score de priorité:** 8.33 (Critical)
**Justification:** Cœur métier de l'application, fonctionnalité principale attendue

## Dependencies
- **Blocks:** #4 (Tableau de Bord), #5 (Alertes), #6 (Sinistres)
- **Blocked by:** #1 (Authentification)

## Implementation Size
- **Estimated effort:** Large (8-10 jours)
- **Sub-issues:**
  - #2.1: Modèles de données et migrations (2 jours)
  - #2.2: Repository et services (2 jours)
  - #2.3: Interface liste et filtres (2 jours)
  - #2.4: Fiche détaillée avec onglets (2 jours)
  - #2.5: Gestion des documents (2 jours)

---

### Issue #3: [CRITICAL] Gestion Complète des Conducteurs

**Labels:** `enhancement`, `critical`, `mvp-core`, `drivers`

## Overview
Module de gestion des conducteurs avec profils détaillés, suivi des permis, affectations de véhicules et historique des infractions.

## Scope
### Inclus
- CRUD complet des conducteurs avec validation
- Profil détaillé multi-onglets (Personnel, Permis, Véhicules, Infractions)
- Gestion des documents (permis, visite médicale)
- Suivi des types de permis et dates d'expiration
- Historique des affectations de véhicules
- Gestion des infractions et points de permis

### Exclus (versions futures)
- Synchronisation avec fichier national des permis
- Géolocalisation des conducteurs
- Évaluation des performances de conduite
- Formation et certifications

## Technical Requirements
- **Entités:** Driver, DriverDocument, DriverLicense, Violation
- **Validation:** Âge minimum, validité des permis, cohérence des dates
- **UI:** Interface Blazor avec upload de photos d'identité
- **Relations:** Liens avec véhicules et historique d'affectations

## Implementation Plan

### Étape 1: Modèles de données
```csharp
public class Driver
{
    public int Id { get; set; }
    [Required, StringLength(50)]
    public string FirstName { get; set; }
    [Required, StringLength(50)]
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DriverStatus Status { get; set; }
    
    // Permis
    public DateTime LicenseObtainedDate { get; set; }
    public DateTime LicenseExpiryDate { get; set; }
    public string LicenseTypes { get; set; } // JSON: ["B", "C1"]
    public int RemainingPoints { get; set; } = 12;
    
    // Navigation
    public ICollection<Vehicle> AssignedVehicles { get; set; }
    public ICollection<DriverDocument> Documents { get; set; }
    public ICollection<Violation> Violations { get; set; }
}
```

### Étape 2: Service d'affectation
```csharp
public class DriverAssignmentService : IDriverAssignmentService
{
    public async Task AssignVehicleAsync(int driverId, int vehicleId)
    {
        var driver = await _driverRepo.GetByIdAsync(driverId);
        var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
        
        // Validation compatibilité permis/véhicule
        ValidateLicenseCompatibility(driver, vehicle);
        
        // Désaffectation précédente si applicable
        await UnassignPreviousVehicle(vehicleId);
        
        // Nouvelle affectation
        vehicle.AssignedDriverId = driverId;
        await _vehicleRepo.UpdateAsync(vehicle);
        
        // Log historique
        await LogAssignmentHistory(driverId, vehicleId);
    }
}
```

### Étape 3: Interface utilisateur
- `/Pages/Drivers/DriversList.razor`
- `/Pages/Drivers/DriverProfile.razor`
- `/Pages/Drivers/CreateDriver.razor`
- `/Components/DriverLicenseManager.razor`

## Acceptance Criteria
- [ ] Liste des conducteurs avec photo, nom, véhicule affecté, statut permis
- [ ] Profil conducteur avec 4 onglets complets
- [ ] Calcul automatique de l'âge et validation âge minimum (18 ans)
- [ ] Upload photo d'identité et documents permis
- [ ] Affectation/désaffectation véhicule avec validation compatibilité
- [ ] Suivi des points de permis avec infractions
- [ ] Alertes pour expiration permis et visite médicale
- [ ] Historique complet des affectations avec dates
- [ ] Export des données conducteurs
- [ ] Tests de validation pour règles métier

## Priority
**Score de priorité:** 8.33 (Critical)
**Justification:** Ressource humaine essentielle, nécessaire pour affectations

## Dependencies
- **Blocks:** #4 (Tableau de Bord), #6 (Sinistres)
- **Blocked by:** #1 (Authentification)

## Implementation Size
- **Estimated effort:** Medium (6-8 jours)
- **Sub-issues:**
  - #3.1: Modèles et validation (2 jours)
  - #3.2: Services d'affectation (2 jours)
  - #3.3: Interface profil détaillé (2 jours)
  - #3.4: Gestion des documents (2 jours)

---

### Issue #4: [CRITICAL] Tableau de Bord Principal

**Labels:** `enhancement`, `critical`, `mvp-core`, `dashboard`

## Overview
Interface principale avec indicateurs clés de performance, alertes urgentes et accès rapide aux fonctionnalités essentielles selon le rôle utilisateur.

## Scope
### Inclus
- KPI principaux : véhicules, conducteurs, sinistres actifs, coûts
- Widget alertes urgentes avec compteurs
- Graphiques évolution kilométrage et coûts mensuels
- Liste véhicules en maintenance
- Derniers sinistres déclarés
- Interface différenciée par rôle (Admin/Gestionnaire/Conducteur)

### Exclus (versions futures)
- Tableaux de bord personnalisables
- Analytics avancées avec BI
- Prédictions et tendances
- Export des rapports dashboard

## Technical Requirements
- **Composants:** Widgets modulaires réutilisables
- **Performance:** Cache des statistiques avec rafraîchissement configurable
- **UI:** Charts avec Chart.js ou similaire
- **Responsive:** Adaptation mobile avec widgets empilés

## Implementation Plan

### Étape 1: Services de statistiques
```csharp
public class DashboardService : IDashboardService
{
    public async Task<DashboardStats> GetStatsAsync()
    {
        return new DashboardStats
        {
            TotalVehicles = await _vehicleRepo.CountAsync(),
            ActiveDrivers = await _driverRepo.CountActiveAsync(),
            ActiveIncidents = await _incidentRepo.CountActiveAsync(),
            VehiclesInMaintenance = await _vehicleRepo.CountInMaintenanceAsync(),
            UrgentAlerts = await _alertService.GetUrgentAlertsAsync(),
            MonthlyMileage = await _vehicleRepo.GetMonthlyMileageAsync(),
            MonthlyCosts = await _costService.GetMonthlyCostsAsync()
        };
    }
}
```

### Étape 2: Composants widgets
```razor
@* /Components/Dashboard/StatsWidget.razor *@
<div class="stats-widget">
    <div class="stat-item">
        <i class="fas fa-car"></i>
        <div class="stat-details">
            <h3>@Stats.TotalVehicles</h3>
            <p>Véhicules</p>
        </div>
    </div>
</div>

@* /Components/Dashboard/AlertsWidget.razor *@
<div class="alerts-widget">
    <h4>Alertes Urgentes (@UrgentAlerts.Count)</h4>
    @foreach(var alert in UrgentAlerts.Take(5))
    {
        <div class="alert-item alert-@alert.Severity.ToLower()">
            <i class="fas @GetAlertIcon(alert.Type)"></i>
            <span>@alert.Message</span>
            <small>@alert.DaysRemaining jours</small>
        </div>
    }
</div>
```

### Étape 3: Dashboard par rôle
- `/Pages/Dashboard/AdminDashboard.razor`
- `/Pages/Dashboard/ManagerDashboard.razor`
- `/Pages/Dashboard/DriverDashboard.razor`

## Acceptance Criteria
- [ ] KPI principaux affichés en temps réel
- [ ] Widget alertes avec compteur et détail des 5 plus urgentes
- [ ] Graphique évolution kilométrage sur 12 mois
- [ ] Graphique répartition des coûts par catégorie
- [ ] Liste des 5 derniers sinistres avec liens vers détails
- [ ] Interface adaptée au rôle utilisateur connecté
- [ ] Temps de chargement < 2 secondes
- [ ] Interface responsive sur tablette
- [ ] Actualisation automatique toutes les 5 minutes
- [ ] Accès rapide aux actions critiques (nouveau sinistre, nouveau véhicule)

## Priority
**Score de priorité:** 4.00 (Critical)
**Justification:** Interface principale, première impression utilisateur

## Dependencies
- **Blocks:** Aucune
- **Blocked by:** #1 (Authentification), #2 (Véhicules), #3 (Conducteurs)

## Implementation Size
- **Estimated effort:** Medium (5-6 jours)
- **Sub-issues:**
  - #4.1: Services de statistiques (2 jours)
  - #4.2: Composants widgets (2 jours)
  - #4.3: Integration et responsive (2 jours)

---

### Issue #5: [CRITICAL] Système d'Alertes Automatiques par Email

**Labels:** `enhancement`, `critical`, `mvp-core`, `alerts`, `notifications`

## Overview
Système d'alertes automatiques par email pour les échéances critiques (CT, assurance, maintenance) avec configuration personnalisable des seuils et destinataires.

## Scope
### Inclus
- Alertes automatiques CT (60, 30, 15 jours avant échéance)
- Alertes assurance (30, 15 jours avant expiration)
- Alertes maintenance basées sur kilométrage ou délai
- Configuration des seuils par type d'alerte
- Templates d'emails personnalisables
- Logs des envois avec statut de livraison

### Exclus (versions futures)
- Notifications SMS
- Push notifications mobile
- Intégration calendriers (Outlook, Google)
- Alertes Slack/Teams

## Technical Requirements
- **Service:** Background service avec planification (Hangfire ou Quartz)
- **Email:** Service SMTP configurable
- **Templates:** Système de templates avec variables dynamiques
- **Configuration:** Interface admin pour paramétrer seuils

## Implementation Plan

### Étape 1: Service d'alertes
```csharp
public class AlertService : IAlertService
{
    public async Task ProcessAlertsAsync()
    {
        await CheckTechnicalInspectionAlerts();
        await CheckInsuranceExpiryAlerts();
        await CheckMaintenanceAlerts();
    }
    
    private async Task CheckTechnicalInspectionAlerts()
    {
        var vehicles = await _vehicleRepo.GetVehiclesWithUpcomingCTAsync();
        
        foreach (var vehicle in vehicles)
        {
            var daysRemaining = (vehicle.NextTechnicalInspection - DateTime.Today).Days;
            var alertSettings = await _settingsRepo.GetCTAlertSettingsAsync();
            
            if (alertSettings.ShouldSendAlert(daysRemaining))
            {
                await SendTechnicalInspectionAlert(vehicle, daysRemaining);
            }
        }
    }
}
```

### Étape 2: Service de notifications
```csharp
public class NotificationService : INotificationService
{
    public async Task SendAlertEmailAsync(AlertEmailDto alertDto)
    {
        var template = await _templateRepo.GetByTypeAsync(alertDto.AlertType);
        var emailBody = await _templateEngine.RenderAsync(template.Content, alertDto);
        
        var emailMessage = new EmailMessage
        {
            To = alertDto.Recipients,
            Subject = template.Subject,
            Body = emailBody,
            IsHtml = true
        };
        
        await _emailService.SendAsync(emailMessage);
        
        // Log de l'envoi
        await _auditService.LogEmailSent(alertDto.AlertType, alertDto.VehicleId);
    }
}
```

### Étape 3: Background job
```csharp
public class AlertBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _alertService.ProcessAlertsAsync();
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken); // Check 4x par jour
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement des alertes");
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}
```

### Étape 4: Interface de configuration
- `/Pages/Admin/AlertSettings.razor`
- `/Components/EmailTemplateEditor.razor`

## Acceptance Criteria
- [ ] Alertes CT envoyées automatiquement à 60, 30 et 15 jours
- [ ] Alertes assurance à 30 et 15 jours avant expiration
- [ ] Email contient toutes les infos : véhicule, conducteur, type échéance, date
- [ ] Liens directs vers fiche véhicule dans l'email
- [ ] Configuration des seuils d'alerte dans interface admin
- [ ] Templates d'emails modifiables avec variables dynamiques
- [ ] Logs des envois avec timestamp et statut de livraison
- [ ] Tests unitaires pour logique de calcul des alertes
- [ ] Gestion des erreurs SMTP avec retry automatique
- [ ] Configuration SMTP sécurisée (TLS/SSL)

## Priority
**Score de priorité:** 5.33 (Critical)
**Justification:** Automatisation clé, prévention des oublis critiques

## Dependencies
- **Blocks:** Aucune
- **Blocked by:** #1 (Authentification), #2 (Véhicules)

## Implementation Size
- **Estimated effort:** Medium (4-5 jours)
- **Sub-issues:**
  - #5.1: Service d'alertes et logique métier (2 jours)
  - #5.2: Service email et templates (1 jour)
  - #5.3: Background service (1 jour)
  - #5.4: Interface configuration (1 jour)

---

## Récapitulatif des Issues Critiques

| Issue | Titre | Priorité | Effort | Dependencies |
|-------|-------|----------|--------|--------------|
| #1 | Authentification et Rôles | 6.25 | 3-5j | Aucune |
| #2 | Gestion Véhicules | 8.33 | 8-10j | #1 |
| #3 | Gestion Conducteurs | 8.33 | 6-8j | #1 |
| #4 | Tableau de Bord | 4.00 | 5-6j | #1, #2, #3 |
| #5 | Alertes Email | 5.33 | 4-5j | #1, #2 |

**Total effort estimé Version 1.0:** 26-34 jours de développement

Cette spécification GitHub Issues fournit un backlog détaillé et prêt pour l'implémentation du MVP avec une approche pragmatique et évolutive.
