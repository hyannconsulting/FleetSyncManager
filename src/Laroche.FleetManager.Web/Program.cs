using Laroche.FleetManager.Infrastructure.Extensions;
using Laroche.FleetManager.Application.Mappings;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Infrastructure.Repositories;
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

// Configuration AutoMapper
builder.Services.AddAutoMapper(typeof(VehicleMappingProfile), typeof(DriverMappingProfile));

// Ajout des services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configuration MediatR pour scanner l'assembly Application
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<Laroche.FleetManager.Application.Queries.Vehicles.GetVehiclesQuery>();
});

// Configuration des repositories
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IVehicleRepository, Laroche.FleetManager.Infrastructure.Repositories.VehicleRepository>();
builder.Services.AddScoped<Laroche.FleetManager.Application.Interfaces.IDriverRepository, Laroche.FleetManager.Infrastructure.Repositories.DriverRepository>();

var app = builder.Build();

// Initialisation de la base de données
try
{
    await app.Services.InitializeDatabaseAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données");
    
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

// Configuration Blazor
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
