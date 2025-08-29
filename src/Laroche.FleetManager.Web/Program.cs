using Laroche.FleetManager.Infrastructure.Extensions;
using Laroche.FleetManager.Application.Mappings;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Infrastructure.Repositories;
using Laroche.FleetManager.Infrastructure.Services;
using Laroche.FleetManager.Infrastructure.Data;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Configuration des services de base de données
builder.Services.AddDatabaseServices(builder.Configuration);

// Configuration ASP.NET Core Identity selon TASK-002
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configuration mots de passe
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configuration verrouillage selon TASK-002 (5 tentatives max)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configuration utilisateur
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    
    // Configuration connexion
    options.SignIn.RequireConfirmedEmail = false; // Simplifier pour l'instant
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuration cookies selon TASK-002 (session 30 minutes)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session 30min selon TASK-002
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Enregistrement des services d'authentification TASK-002
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ILoginAuditService, LoginAuditService>();

// Configuration AutoMapper
builder.Services.AddAutoMapper(typeof(VehicleMappingProfile), typeof(DriverMappingProfile));

// Ajout des services
builder.Services.AddRazorPages(options =>
{
    // Configuration autorisation par rôles selon TASK-002
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});
builder.Services.AddServerSideBlazor();

// Configuration politiques d'autorisation selon TASK-002
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole(UserRoles.Admin));
    
    options.AddPolicy("FleetManagerPolicy", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.FleetManager));
    
    options.AddPolicy("DriverPolicy", policy =>
        policy.RequireRole(UserRoles.Admin, UserRoles.FleetManager, UserRoles.Driver));
});

// Configuration MediatR pour scanner l'assembly Application
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<Laroche.FleetManager.Application.Queries.Vehicles.GetVehiclesQuery>();
});

// Configuration des repositories
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IVehicleRepository, Laroche.FleetManager.Infrastructure.Repositories.VehicleRepository>();
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IDriverRepository, Laroche.FleetManager.Infrastructure.Repositories.DriverRepository>();

// Configuration des services
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IAuthenticationService, Laroche.FleetManager.Infrastructure.Services.AuthenticationService>();
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.ILoginAuditService, Laroche.FleetManager.Infrastructure.Services.LoginAuditService>();
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IUserService, Laroche.FleetManager.Infrastructure.Services.UserService>();

var app = builder.Build();

// Initialisation de la base de données
try
{
    await app.Services.InitializeDatabaseAsync();
    
    // Initialisation des rôles et utilisateur admin TASK-002
    await AuthenticationSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données ou de l'authentification");
    
    if (app.Environment.IsDevelopment())
    {
        throw; // Re-lancer l'exception en développement
    }
}

// Configuration des middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configuration middleware Identity selon TASK-002
app.UseAuthentication(); // IMPORTANT: doit être avant UseAuthorization
app.UseAuthorization();

// Configuration Blazor
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
