using Laroche.FleetManager.API.Extensions;
using Laroche.FleetManager.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("🚀 Démarrage de FleetSyncManager API...");

    // Configuration des services
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() 
        { 
            Title = "FleetSyncManager API", 
            Version = "v1",
            Description = "API REST pour la gestion de flotte automobile",
            Contact = new()
            {
                Name = "FleetSyncManager Team",
                Email = "support@fleetsynmanager.com"
            }
        });
        
        // Documentation XML
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });

    // TODO: Enregistrement des couches (à implémenter plus tard)
    // builder.Services.AddApplicationServices();
    // builder.Services.AddInfrastructureServices(builder.Configuration);

    // Configuration API spécifique
    builder.Services.AddApiServices();

    // Configuration CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorClient", policy =>
        {
            policy
                .WithOrigins("https://localhost:7001", "http://localhost:5001") // Ports du projet Web
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Health Checks (simplifié)
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configuration du pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetSyncManager API v1");
            c.RoutePrefix = string.Empty; // Swagger à la racine
        });
    }

    // Middleware personnalisé
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    
    app.UseHttpsRedirection();
    app.UseCors("AllowBlazorClient");
    
    // Health Checks endpoints
    app.MapHealthChecks("/health");

    // Configuration des endpoints minimalistes
    app.ConfigureApiEndpoints();

    Log.Information("✅ FleetSyncManager API configurée avec succès");
    Log.Information("📖 Documentation Swagger disponible sur : {SwaggerUrl}", 
        app.Environment.IsDevelopment() ? "https://localhost:7002" : "N/A");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Erreur fatale lors du démarrage de l'API");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
