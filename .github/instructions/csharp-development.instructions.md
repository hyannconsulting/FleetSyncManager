---
applyTo: "**/*.cs"
---
# Instructions de Développement C# - FleetSyncManager

Appliquer les [directives générales de codage](./general-coding.instructions.md) à tout le code C#.

## Standards Spécifiques C# et ASP.NET Core

### Conventions de Nommage C#
- **Classes et Interfaces :** PascalCase (`VehicleService`, `IVehicleRepository`)
- **Méthodes et Propriétés :** PascalCase (`GetVehicleById`, `LicensePlate`)
- **Variables et Paramètres :** camelCase (`vehicleId`, `currentMileage`)
- **Champs privés :** camelCase avec underscore (`_vehicleRepository`, `_mapper`)
- **Constantes :** ALL_CAPS avec underscores (`MAX_UPLOAD_SIZE`, `DEFAULT_PAGE_SIZE`)
- **Enums :** PascalCase pour l'enum et les valeurs (`VehicleStatus.Active`)

### Architecture Clean Architecture
```csharp
// Structure des namespaces
FleetSync.Core.Entities          // Domain entities
FleetSync.Core.Enums            // Domain enumerations  
FleetSync.Core.Interfaces       // Domain contracts
FleetSync.Application.Services  // Business logic
FleetSync.Application.DTOs      // Data transfer objects
FleetSync.Infrastructure.Data   // Data access implementation
FleetSync.Web.Pages            // Blazor pages
FleetSync.Web.Components       // Blazor components
```

### Entités et Domain Models
```csharp
// Exemple d'entité domain
public class Vehicle : BaseEntity
{
    [Required, StringLength(20)]
    public string LicensePlate { get; set; }
    
    [StringLength(17)]
    public string Vin { get; set; }
    
    [Required, StringLength(50)]
    public string Brand { get; set; }
    
    public VehicleStatus Status { get; set; }
    
    // Navigation properties
    public int? AssignedDriverId { get; set; }
    public Driver? AssignedDriver { get; set; }
    public ICollection<VehicleDocument> Documents { get; set; } = new List<VehicleDocument>();
    
    // Business methods
    public bool IsMaintenanceDue()
    {
        return (NextMaintenanceDate.HasValue && NextMaintenanceDate <= DateTime.Today) ||
               (NextMaintenanceMileage.HasValue && CurrentMileage >= NextMaintenanceMileage);
    }
}
```

### Services et Injection de Dépendances
```csharp
// Interface de service
public interface IVehicleService
{
    Task<PagedResult<VehicleDto>> GetVehiclesAsync(VehicleFilterDto filter);
    Task<VehicleDetailsDto> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);
    Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
    Task DeleteVehicleAsync(int id);
}

// Implémentation de service
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<VehicleService> _logger;
    
    public VehicleService(
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILogger<VehicleService> logger)
    {
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        
        try
        {
            await ValidateVehicleDataAsync(dto);
            
            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.CreatedAt = DateTime.UtcNow;
            
            var createdVehicle = await _vehicleRepository.CreateAsync(vehicle);
            
            _logger.LogInformation("Véhicule créé avec succès: {LicensePlate}", vehicle.LicensePlate);
            
            return _mapper.Map<VehicleDto>(createdVehicle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du véhicule");
            throw;
        }
    }
}
```

### Repository Pattern avec Entity Framework
```csharp
// Interface repository générique
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null);
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Repository spécialisé
public interface IVehicleRepository : IGenericRepository<Vehicle>
{
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<Vehicle>> GetVehiclesWithUpcomingInspectionAsync(int daysAhead = 60);
    Task<bool> IsLicensePlateUniqueAsync(string licensePlate, int? excludeId = null);
}

// Implémentation repository
public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _dbSet
            .Include(v => v.AssignedDriver)
            .Include(v => v.Documents)
            .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);
    }
    
    public async Task<bool> IsLicensePlateUniqueAsync(string licensePlate, int? excludeId = null)
    {
        var query = _dbSet.Where(v => v.LicensePlate == licensePlate);
        
        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);
            
        return !await query.AnyAsync();
    }
}
```

### DTOs et Validation
```csharp
// DTO de création
public class CreateVehicleDto
{
    [Required(ErrorMessage = "L'immatriculation est obligatoire")]
    [StringLength(20, ErrorMessage = "L'immatriculation ne peut pas dépasser 20 caractères")]
    public string LicensePlate { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La marque est obligatoire")]
    [StringLength(50, ErrorMessage = "La marque ne peut pas dépasser 50 caractères")]
    public string Brand { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Le modèle est obligatoire")]
    [StringLength(50, ErrorMessage = "Le modèle ne peut pas dépasser 50 caractères")]
    public string Model { get; set; } = string.Empty;
    
    [Range(1900, 2030, ErrorMessage = "L'année doit être comprise entre 1900 et 2030")]
    public int Year { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Le kilométrage ne peut pas être négatif")]
    public int CurrentMileage { get; set; }
    
    public VehicleStatus Status { get; set; } = VehicleStatus.Active;
}

// Validator FluentValidation (optionnel pour validations complexes)
public class CreateVehicleDtoValidator : AbstractValidator<CreateVehicleDto>
{
    private readonly IVehicleRepository _vehicleRepository;
    
    public CreateVehicleDtoValidator(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
        
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("L'immatriculation est obligatoire")
            .Length(1, 20).WithMessage("L'immatriculation doit contenir entre 1 et 20 caractères")
            .MustAsync(BeUniqueLicensePlate).WithMessage("Cette immatriculation existe déjà");
            
        RuleFor(x => x.Year)
            .GreaterThan(1950).WithMessage("L'année doit être supérieure à 1950")
            .LessThanOrEqualTo(DateTime.Now.Year + 2).WithMessage("L'année ne peut pas être dans un futur lointain");
    }
    
    private async Task<bool> BeUniqueLicensePlate(string licensePlate, CancellationToken token)
    {
        return await _vehicleRepository.IsLicensePlateUniqueAsync(licensePlate);
    }
}
```

### Configuration Entity Framework
```csharp
// Configuration d'entité
public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        
        // Clé primaire
        builder.HasKey(v => v.Id);
        
        // Index
        builder.HasIndex(v => v.LicensePlate)
            .IsUnique()
            .HasDatabaseName("IX_Vehicles_LicensePlate");
            
        // Propriétés
        builder.Property(v => v.LicensePlate)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);
            
        builder.Property(v => v.Brand)
            .IsRequired()
            .HasMaxLength(50);
            
        // Enum stocké comme integer
        builder.Property(v => v.Status)
            .HasConversion<int>();
            
        // Relations
        builder.HasOne(v => v.AssignedDriver)
            .WithMany(d => d.AssignedVehicles)
            .HasForeignKey(v => v.AssignedDriverId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(v => v.Documents)
            .WithOne(d => d.Vehicle)
            .HasForeignKey(d => d.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

// DbContext
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Appliquer toutes les configurations
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Contraintes globales
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            // Propriétés DateTime en UTC par défaut
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamptz");
                }
            }
        }
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Audit automatique
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
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
    }
}
```

### Gestion des Exceptions
```csharp
// Exceptions métier personnalisées
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
    
    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key) 
        : base($"L'entité '{entityName}' avec la clé '{key}' n'a pas été trouvée.")
    {
    }
}

// Middleware de gestion d'exceptions
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Une erreur non gérée s'est produite");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResponse();
        
        switch (exception)
        {
            case BusinessException:
                response.Message = exception.Message;
                context.Response.StatusCode = 400;
                break;
            case NotFoundException:
                response.Message = exception.Message;
                context.Response.StatusCode = 404;
                break;
            default:
                response.Message = "Une erreur interne s'est produite.";
                context.Response.StatusCode = 500;
                break;
        }
        
        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
```

### Configuration des Services (Program.cs)
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuration de l'authentification
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configuration AutoMapper
builder.Services.AddAutoMapper(typeof(VehicleMappingProfile));

// Enregistrement des services
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Configuration de la validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleDtoValidator>();

// Configuration Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configuration du logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Pipeline de traitement des requêtes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Seed des données
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context, scope.ServiceProvider);
}

app.Run();
```

### Tests Unitaires
```csharp
[TestFixture]
public class VehicleServiceTests
{
    private Mock<IVehicleRepository> _mockVehicleRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<VehicleService>> _mockLogger;
    private VehicleService _vehicleService;
    
    [SetUp]
    public void SetUp()
    {
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<VehicleService>>();
        
        _vehicleService = new VehicleService(
            _mockVehicleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object);
    }
    
    [Test]
    public async Task CreateVehicleAsync_WithValidDto_ShouldReturnVehicleDto()
    {
        // Arrange
        var createDto = new CreateVehicleDto
        {
            LicensePlate = "AB-123-CD",
            Brand = "Peugeot",
            Model = "308",
            Year = 2020
        };
        
        var vehicle = new Vehicle { Id = 1, LicensePlate = "AB-123-CD" };
        var vehicleDto = new VehicleDto { Id = 1, LicensePlate = "AB-123-CD" };
        
        _mockMapper.Setup(m => m.Map<Vehicle>(createDto)).Returns(vehicle);
        _mockVehicleRepository.Setup(r => r.CreateAsync(vehicle)).ReturnsAsync(vehicle);
        _mockMapper.Setup(m => m.Map<VehicleDto>(vehicle)).Returns(vehicleDto);
        
        // Act
        var result = await _vehicleService.CreateVehicleAsync(createDto);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.LicensePlate, Is.EqualTo("AB-123-CD"));
        _mockVehicleRepository.Verify(r => r.CreateAsync(It.IsAny<Vehicle>()), Times.Once);
    }
    
    [Test]
    public void CreateVehicleAsync_WithNullDto_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _vehicleService.CreateVehicleAsync(null));
    }
}
```

## Bonnes Pratiques Spécifiques

### Performance
- **Async/Await :** Utiliser systématiquement pour les opérations I/O
- **ConfigureAwait(false) :** Dans les bibliothèques pour éviter les deadlocks
- **IAsyncEnumerable :** Pour les grandes collections
- **Pagination :** Toujours implémenter pour les listes

### Sécurité
- **Validation :** Double validation côté client et serveur
- **Sanitization :** Nettoyer les entrées utilisateur
- **Authorization :** Vérifier les permissions à tous les niveaux
- **SQL Injection :** Utiliser uniquement des requêtes paramétrées avec EF Core

### Maintenabilité
- **SOLID :** Respecter les principes SOLID
- **DRY :** Ne pas répéter le code
- **Separation of Concerns :** Une responsabilité par classe
- **Interface Segregation :** Interfaces petites et cohérentes

Ces instructions assurent un développement C# cohérent et de qualité pour le projet FleetSyncManager.
