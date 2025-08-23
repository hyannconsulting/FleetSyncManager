# Architecture Technique MVP - FleetSyncManager

## Vue d'Ensemble de l'Architecture

### Stack Technique Principal
- **Backend Framework:** ASP.NET Core 8.0
- **Frontend:** Blazor Server
- **Base de données:** PostgreSQL 15+
- **ORM:** Entity Framework Core 8.0
- **Authentication:** ASP.NET Core Identity
- **File Storage:** Système de fichiers local (migration Azure Blob prévue)
- **Email Service:** SMTP configurable
- **Background Jobs:** Background Services .NET
- **Documentation API:** OpenAPI/Swagger

### Architecture Pattern
**Clean Architecture** avec séparation des préoccupations :
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Presentation  │    │   Application   │    │  Infrastructure │
│   (Blazor UI)   │◄──►│   (Business)    │◄──►│  (Data/Email)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
           │                       │                       │
           └───────────────────────┼───────────────────────┘
                                   ▼
                           ┌─────────────────┐
                           │      Core       │
                           │   (Entities)    │
                           └─────────────────┘
```

## Structure du Projet

### Organisation des Solutions
```
FleetSyncManager.sln
├── src/
│   ├── FleetSync.Web/                 # Blazor Server App
│   │   ├── Pages/                     # Pages Blazor
│   │   ├── Components/                # Composants réutilisables
│   │   ├── wwwroot/                   # Assets statiques
│   │   └── Program.cs                 # Configuration app
│   │
│   ├── FleetSync.API/                 # Web API (futur mobile)
│   │   ├── Controllers/               # API Controllers
│   │   ├── Middleware/                # Custom middleware
│   │   └── Program.cs                 # API Configuration
│   │
│   ├── FleetSync.Application/         # Business Logic
│   │   ├── Services/                  # Services métier
│   │   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Validators/                # FluentValidation
│   │   ├── Mappings/                  # AutoMapper profiles
│   │   └── Interfaces/                # Contracts
│   │
│   ├── FleetSync.Core/                # Domain Layer
│   │   ├── Entities/                  # Domain Models
│   │   ├── Enums/                     # Enumerations
│   │   ├── ValueObjects/              # Value Objects
│   │   └── Interfaces/                # Domain Contracts
│   │
│   ├── FleetSync.Infrastructure/      # Data & External Services
│   │   ├── Data/                      # EF Context & Repositories
│   │   ├── Services/                  # External Services
│   │   ├── Migrations/                # EF Migrations
│   │   └── Configurations/            # EF Configurations
│   │
│   └── FleetSync.Shared/              # Shared Components
│       ├── Constants/                 # Application Constants
│       ├── Extensions/                # Extension Methods
│       └── Helpers/                   # Utility Classes
│
├── tests/
│   ├── FleetSync.UnitTests/           # Tests unitaires
│   ├── FleetSync.IntegrationTests/    # Tests d'intégration
│   └── FleetSync.E2ETests/            # Tests end-to-end
│
├── database/
│   ├── scripts/                       # Scripts SQL
│   └── seed-data/                     # Données de test
│
└── docs/
    ├── api/                           # Documentation API
    └── deployment/                    # Guides déploiement
```

## Modèles de Données (Core Domain)

### Entités Principales

#### Vehicle Entity
```csharp
namespace FleetSync.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        // Identification
        public string LicensePlate { get; set; } // Immatriculation (unique)
        public string Vin { get; set; } // Numéro de châssis
        public string Brand { get; set; } // Marque
        public string Model { get; set; } // Modèle
        public int Year { get; set; } // Année
        public string Color { get; set; } // Couleur
        
        // Caractéristiques techniques
        public FuelType FuelType { get; set; } // Carburant
        public int EngineSize { get; set; } // Cylindrée (cm3)
        public int Power { get; set; } // Puissance (CV)
        public VehicleCategory Category { get; set; } // VP, VUL, PL
        
        // État et utilisation
        public VehicleStatus Status { get; set; } // Actif, Maintenance, Réformé
        public int CurrentMileage { get; set; } // Kilométrage actuel
        public DateTime LastMileageUpdate { get; set; }
        
        // Dates importantes
        public DateTime PurchaseDate { get; set; } // Date d'acquisition
        public DateTime? WarrantyEndDate { get; set; } // Fin garantie
        public DateTime NextTechnicalInspection { get; set; } // Prochain CT
        public DateTime InsuranceExpiryDate { get; set; } // Fin assurance
        public DateTime? NextMaintenanceDate { get; set; } // Prochain entretien
        public int? NextMaintenanceMileage { get; set; } // Prochain entretien (km)
        
        // Relations
        public int? AssignedDriverId { get; set; }
        public Driver? AssignedDriver { get; set; }
        public ICollection<VehicleDocument> Documents { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
        public ICollection<Incident> Incidents { get; set; }
        public ICollection<MileageRecord> MileageHistory { get; set; }
        
        // Méthodes métier
        public bool IsMaintenanceDue() => 
            (NextMaintenanceDate.HasValue && NextMaintenanceDate <= DateTime.Today) ||
            (NextMaintenanceMileage.HasValue && CurrentMileage >= NextMaintenanceMileage);
            
        public int DaysUntilTechnicalInspection() =>
            (NextTechnicalInspection - DateTime.Today).Days;
            
        public bool HasUrgentAlert() =>
            DaysUntilTechnicalInspection() <= 15 ||
            (InsuranceExpiryDate - DateTime.Today).Days <= 15 ||
            IsMaintenanceDue();
    }
}
```

#### Driver Entity
```csharp
namespace FleetSync.Core.Entities
{
    public class Driver : BaseEntity
    {
        // Informations personnelles
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
        
        // Contact d'urgence
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string EmergencyContactRelation { get; set; }
        
        // Statut
        public DriverStatus Status { get; set; } // Actif, Suspendu, Inactif
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        
        // Permis de conduire
        public DateTime LicenseObtainedDate { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseTypes { get; set; } // JSON: ["B", "C1", "D"]
        public int RemainingPoints { get; set; } = 12;
        
        // Suivi médical
        public DateTime? LastMedicalCheckDate { get; set; }
        public DateTime? NextMedicalCheckDate { get; set; }
        
        // Relations
        public ICollection<Vehicle> AssignedVehicles { get; set; }
        public ICollection<DriverDocument> Documents { get; set; }
        public ICollection<Violation> Violations { get; set; }
        public ICollection<Incident> Incidents { get; set; }
        
        // Propriétés calculées
        public int Age => DateTime.Today.Year - DateOfBirth.Year - 
            (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
            
        public string FullName => $"{FirstName} {LastName}";
        
        public bool IsLicenseExpiringSoon() =>
            (LicenseExpiryDate - DateTime.Today).Days <= 30;
            
        public bool CanDriveVehicle(VehicleCategory category) =>
            category switch
            {
                VehicleCategory.Car => LicenseTypes.Contains("B"),
                VehicleCategory.LightTruck => LicenseTypes.Contains("C1") || LicenseTypes.Contains("C"),
                VehicleCategory.HeavyTruck => LicenseTypes.Contains("C"),
                VehicleCategory.Bus => LicenseTypes.Contains("D"),
                _ => false
            };
    }
}
```

#### Incident Entity
```csharp
namespace FleetSync.Core.Entities
{
    public class Incident : BaseEntity
    {
        // Identification
        public string IncidentNumber { get; set; } // Généré automatiquement
        public IncidentType Type { get; set; } // Accident, Vol, Panne, etc.
        public IncidentSeverity Severity { get; set; } // Mineur, Majeur, Critique
        public IncidentStatus Status { get; set; } // Déclaré, EnCours, Clos
        
        // Circonstances
        public DateTime IncidentDate { get; set; }
        public TimeSpan IncidentTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string WeatherConditions { get; set; }
        public string RoadConditions { get; set; }
        
        // Responsabilité
        public bool DriverAtFault { get; set; }
        public string FaultDescription { get; set; }
        
        // Relations
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        
        // Documents et suivi
        public ICollection<IncidentDocument> Documents { get; set; }
        public ICollection<IncidentNote> Notes { get; set; }
        
        // Coûts
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }
        public decimal Deductible { get; set; } // Franchise
        public string InsuranceClaimNumber { get; set; }
        
        // Tiers impliqués
        public bool ThirdPartyInvolved { get; set; }
        public string ThirdPartyDetails { get; set; }
        public string WitnessDetails { get; set; }
        
        // Méthodes métier
        public bool IsUrgent() => Severity == IncidentSeverity.Critical;
        public bool IsOpen() => Status != IncidentStatus.Closed;
        public int DaysSinceIncident() => (DateTime.Today - IncidentDate.Date).Days;
    }
}
```

### Enums et Value Objects

#### Enumerations
```csharp
namespace FleetSync.Core.Enums
{
    public enum VehicleStatus
    {
        Active = 1,
        Maintenance = 2,
        OutOfOrder = 3,
        Retired = 4
    }
    
    public enum VehicleCategory
    {
        Car = 1,           // Véhicule particulier
        LightTruck = 2,    // Véhicule utilitaire léger
        HeavyTruck = 3,    // Poids lourd
        Bus = 4,           // Autocar/Bus
        Motorcycle = 5     // Deux roues
    }
    
    public enum FuelType
    {
        Gasoline = 1,      // Essence
        Diesel = 2,        // Diesel
        Electric = 3,      // Électrique
        Hybrid = 4,        // Hybride
        LPG = 5,          // GPL
        CNG = 6           // GNV
    }
    
    public enum DriverStatus
    {
        Active = 1,
        Suspended = 2,
        OnLeave = 3,
        Terminated = 4
    }
    
    public enum IncidentType
    {
        Accident = 1,      // Accident de circulation
        Theft = 2,         // Vol
        Vandalism = 3,     // Vandalisme
        Breakdown = 4,     // Panne
        Fire = 5,          // Incendie
        NaturalDisaster = 6 // Catastrophe naturelle
    }
    
    public enum IncidentSeverity
    {
        Minor = 1,         // Mineur (dégâts < 1000€)
        Major = 2,         // Majeur (dégâts 1000-5000€)
        Critical = 3       // Critique (dégâts > 5000€ ou blessés)
    }
    
    public enum AlertType
    {
        TechnicalInspection = 1,
        InsuranceExpiry = 2,
        MaintenanceDue = 3,
        LicenseExpiry = 4,
        MedicalCheckDue = 5
    }
}
```

## Architecture des Services

### Service Layer Pattern

#### Interface Contracts
```csharp
namespace FleetSync.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleDto>> GetVehiclesAsync(VehicleFilterDto filter);
        Task<VehicleDetailsDto> GetVehicleByIdAsync(int id);
        Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);
        Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
        Task DeleteVehicleAsync(int id);
        Task<bool> AssignDriverAsync(int vehicleId, int driverId);
        Task<bool> UnassignDriverAsync(int vehicleId);
        Task UpdateMileageAsync(int vehicleId, int newMileage, string photoUrl);
        Task<IEnumerable<AlertDto>> GetVehicleAlertsAsync(int vehicleId);
    }
    
    public interface IDriverService
    {
        Task<PagedResult<DriverDto>> GetDriversAsync(DriverFilterDto filter);
        Task<DriverDetailsDto> GetDriverByIdAsync(int id);
        Task<DriverDto> CreateDriverAsync(CreateDriverDto dto);
        Task<DriverDto> UpdateDriverAsync(int id, UpdateDriverDto dto);
        Task DeleteDriverAsync(int id);
        Task<bool> ValidateLicenseCompatibilityAsync(int driverId, int vehicleId);
        Task<IEnumerable<VehicleDto>> GetDriverVehicleHistoryAsync(int driverId);
    }
    
    public interface IIncidentService
    {
        Task<PagedResult<IncidentDto>> GetIncidentsAsync(IncidentFilterDto filter);
        Task<IncidentDetailsDto> GetIncidentByIdAsync(int id);
        Task<IncidentDto> CreateIncidentAsync(CreateIncidentDto dto);
        Task<IncidentDto> UpdateIncidentStatusAsync(int id, IncidentStatus newStatus);
        Task<string> GenerateIncidentNumberAsync();
        Task<IEnumerable<IncidentStatisticsDto>> GetIncidentStatisticsAsync(DateTime from, DateTime to);
    }
    
    public interface IAlertService
    {
        Task ProcessAllAlertsAsync();
        Task<IEnumerable<AlertDto>> GetActiveAlertsAsync();
        Task<IEnumerable<AlertDto>> GetUrgentAlertsAsync();
        Task MarkAlertAsProcessedAsync(int alertId);
        Task ConfigureAlertSettingsAsync(AlertSettingsDto settings);
    }
}
```

#### Service Implementations
```csharp
namespace FleetSync.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VehicleService> _logger;
        
        public VehicleService(
            IVehicleRepository vehicleRepository,
            IDriverRepository driverRepository,
            IMapper mapper,
            ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
        {
            // Validation métier
            await ValidateVehicleDataAsync(dto);
            
            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.CreatedAt = DateTime.UtcNow;
            
            // Configuration automatique des alertes
            ConfigureAutomaticAlerts(vehicle);
            
            vehicle = await _vehicleRepository.CreateAsync(vehicle);
            
            _logger.LogInformation("Véhicule créé: {LicensePlate}", vehicle.LicensePlate);
            
            return _mapper.Map<VehicleDto>(vehicle);
        }
        
        private async Task ValidateVehicleDataAsync(CreateVehicleDto dto)
        {
            // Vérification unicité immatriculation
            var existingVehicle = await _vehicleRepository.GetByLicensePlateAsync(dto.LicensePlate);
            if (existingVehicle != null)
                throw new BusinessException($"Un véhicule avec l'immatriculation {dto.LicensePlate} existe déjà");
            
            // Validation des dates
            if (dto.NextTechnicalInspection <= DateTime.Today)
                throw new BusinessException("La date du prochain contrôle technique doit être dans le futur");
            
            if (dto.InsuranceExpiryDate <= DateTime.Today)
                throw new BusinessException("La date d'expiration de l'assurance doit être dans le futur");
        }
        
        private void ConfigureAutomaticAlerts(Vehicle vehicle)
        {
            // Configuration des alertes selon les échéances
            // Cette logique sera utilisée par le service d'alertes
            vehicle.NextMaintenanceDate = CalculateNextMaintenanceDate(vehicle);
            vehicle.NextMaintenanceMileage = CalculateNextMaintenanceMileage(vehicle);
        }
    }
}
```

## Couche d'Accès aux Données

### Repository Pattern avec Entity Framework

#### Generic Repository
```csharp
namespace FleetSync.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        
        public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate = null)
        {
            var query = _dbSet.AsQueryable();
            
            if (predicate != null)
                query = query.Where(predicate);
            
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        
        public async Task<T> CreateAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        
        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

#### Specialized Repositories
```csharp
namespace FleetSync.Infrastructure.Data.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context) { }
        
        public async Task<Vehicle> GetByLicensePlateAsync(string licensePlate)
        {
            return await _dbSet
                .Include(v => v.AssignedDriver)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
        }
        
        public async Task<IEnumerable<Vehicle>> GetVehiclesWithUpcomingInspectionAsync(int daysAhead = 60)
        {
            var cutoffDate = DateTime.Today.AddDays(daysAhead);
            return await _dbSet
                .Include(v => v.AssignedDriver)
                .Where(v => v.NextTechnicalInspection <= cutoffDate && v.Status == VehicleStatus.Active)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Vehicle>> GetVehiclesWithExpiringInsuranceAsync(int daysAhead = 30)
        {
            var cutoffDate = DateTime.Today.AddDays(daysAhead);
            return await _dbSet
                .Include(v => v.AssignedDriver)
                .Where(v => v.InsuranceExpiryDate <= cutoffDate && v.Status == VehicleStatus.Active)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Vehicle>> GetVehiclesInMaintenanceAsync()
        {
            return await _dbSet
                .Include(v => v.AssignedDriver)
                .Where(v => v.Status == VehicleStatus.Maintenance)
                .ToListAsync();
        }
        
        public async Task<VehicleStatistics> GetVehicleStatisticsAsync()
        {
            return new VehicleStatistics
            {
                TotalVehicles = await _dbSet.CountAsync(),
                ActiveVehicles = await _dbSet.CountAsync(v => v.Status == VehicleStatus.Active),
                VehiclesInMaintenance = await _dbSet.CountAsync(v => v.Status == VehicleStatus.Maintenance),
                TotalMileage = await _dbSet.SumAsync(v => v.CurrentMileage),
                AverageAge = await _dbSet.AverageAsync(v => DateTime.Now.Year - v.Year)
            };
        }
    }
}
```

### Entity Framework Configuration

#### DbContext Configuration
```csharp
namespace FleetSync.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        // DbSets
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<VehicleDocument> VehicleDocuments { get; set; }
        public DbSet<DriverDocument> DriverDocuments { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Application des configurations
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Seed des données de base
            SeedData(builder);
        }
        
        private void SeedData(ModelBuilder builder)
        {
            // Seed des rôles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new IdentityRole { Id = "2", Name = "FleetManager", NormalizedName = "FLEETMANAGER" },
                new IdentityRole { Id = "3", Name = "Driver", NormalizedName = "DRIVER" }
            );
        }
        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Audit automatique des entités
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
```

#### Entity Configurations
```csharp
namespace FleetSync.Infrastructure.Data.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            
            // Clé primaire
            builder.HasKey(v => v.Id);
            
            // Index et contraintes
            builder.HasIndex(v => v.LicensePlate).IsUnique();
            builder.HasIndex(v => v.Vin).IsUnique();
            
            // Propriétés
            builder.Property(v => v.LicensePlate)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(v => v.Vin)
                .HasMaxLength(17);
                
            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(50);
            
            // Relations
            builder.HasOne(v => v.AssignedDriver)
                .WithMany(d => d.AssignedVehicles)
                .HasForeignKey(v => v.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);
                
            builder.HasMany(v => v.Documents)
                .WithOne(d => d.Vehicle)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(v => v.MaintenanceRecords)
                .WithOne(m => m.Vehicle)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");
            
            builder.HasKey(d => d.Id);
            
            // Index
            builder.HasIndex(d => d.Email).IsUnique();
            builder.HasIndex(d => d.LicenseNumber).IsUnique();
            
            // Propriétés
            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(d => d.LicenseTypes)
                .HasMaxLength(100); // JSON array stocké comme string
            
            // Relations
            builder.HasMany(d => d.Documents)
                .WithOne(doc => doc.Driver)
                .HasForeignKey(doc => doc.DriverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```

Cette architecture technique fournit une base solide et évolutive pour le développement du MVP FleetSyncManager, avec une séparation claire des responsabilités et des patterns éprouvés.
