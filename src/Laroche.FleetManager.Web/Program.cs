using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.Behaviors;
using Laroche.FleetManager.Application.Commands.Vehicles;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Laroche.FleetManager.Application.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Reflection;

namespace Laroche.FleetManager.Web;

/// <summary>
/// Main program entry point
/// </summary>
public class Program
{
    /// <summary>
    /// Main method
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/fleetmanager-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Starting FleetSyncManager application");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Configure services
            ConfigureServices(builder);
            
            var app = builder.Build();
            
            // Configure pipeline
            await ConfigurePipeline(app);
            
            // Configure endpoints
            ConfigureEndpoints(app);
            
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Configure application services
    /// </summary>
    /// <param name="builder">Web application builder</param>
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Add Serilog
        builder.Host.UseSerilog();
        
        // Add Blazor Server
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        
        // Add API services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "FleetSyncManager API",
                Version = "v1",
                Description = "API for Fleet Management System"
            });
            
            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
        
        // Add MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateVehicleCommand).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
        
        // Add FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(CreateVehicleCommand).Assembly);
        
        // Add AutoMapper
        builder.Services.AddAutoMapper(typeof(CreateVehicleCommand).Assembly);
        
        // Add HttpContext accessor
        builder.Services.AddHttpContextAccessor();
        
        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // TODO: Add Entity Framework, Authentication, etc.
        // This will be added in the Infrastructure layer setup
    }

    /// <summary>
    /// Configure application pipeline
    /// </summary>
    /// <param name="app">Web application</param>
    private static async Task ConfigurePipeline(WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetSyncManager API v1");
                c.RoutePrefix = "api-docs";
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors("AllowAll");
        
        // TODO: Add Authentication & Authorization
        // app.UseAuthentication();
        // app.UseAuthorization();

        app.MapRazorPages();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Configure API endpoints
    /// </summary>
    /// <param name="app">Web application</param>
    private static void ConfigureEndpoints(WebApplication app)
    {
        var apiGroup = app.MapGroup("/api/v1")
            .WithTags("API v1")
            .WithOpenApi();

        // Vehicle endpoints
        ConfigureVehicleEndpoints(apiGroup);
        
        // TODO: Add other entity endpoints (Drivers, Maintenance, etc.)
    }

    /// <summary>
    /// Configure vehicle endpoints
    /// </summary>
    /// <param name="group">Route group builder</param>
    private static void ConfigureVehicleEndpoints(RouteGroupBuilder group)
    {
        var vehicleGroup = group.MapGroup("/vehicles")
            .WithTags("Vehicles");

        // GET /api/v1/vehicles
        vehicleGroup.MapGet("/", async (
            [FromServices] IMediator mediator,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? brand = null,
            [FromQuery] string? status = null,
            [FromQuery] string? fuelType = null,
            [FromQuery] string sortBy = "LicensePlate",
            [FromQuery] string sortDirection = "asc") =>
        {
            var query = new GetVehiclesQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Brand = brand,
                Status = status,
                FuelType = fuelType,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await mediator.Send(query);
            
            return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("GetVehicles")
        .WithSummary("Get all vehicles with pagination and filtering")
        .Produces<PagedResult<VehicleDto>>();

        // GET /api/v1/vehicles/{id}
        vehicleGroup.MapGet("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator) =>
        {
            var query = new GetVehicleByIdQuery(id);
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
                return Results.BadRequest(result.ErrorMessage);

            return result.Data != null ? Results.Ok(result.Data) : Results.NotFound();
        })
        .WithName("GetVehicleById")
        .WithSummary("Get vehicle by ID")
        .Produces<VehicleDto>()
        .Produces(404);

        // POST /api/v1/vehicles
        vehicleGroup.MapPost("/", async (
            [FromBody] CreateVehicleCommand command,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            
            if (!result.IsSuccess)
            {
                if (result.ValidationErrors.Any())
                    return Results.BadRequest(result.ValidationErrors);
                return Results.BadRequest(result.ErrorMessage);
            }

            return Results.CreatedAtRoute("GetVehicleById", new { id = result.Data!.Id }, result.Data);
        })
        .WithName("CreateVehicle")
        .WithSummary("Create a new vehicle")
        .Produces<VehicleDto>(201)
        .Produces(400);

        // PUT /api/v1/vehicles/{id}
        vehicleGroup.MapPut("/{id:int}", async (
            [FromRoute] int id,
            [FromBody] UpdateVehicleCommand command,
            [FromServices] IMediator mediator) =>
        {
            command.Id = id;
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.ValidationErrors.Any())
                    return Results.BadRequest(result.ValidationErrors);
                return Results.BadRequest(result.ErrorMessage);
            }

            return Results.Ok(result.Data);
        })
        .WithName("UpdateVehicle")
        .WithSummary("Update an existing vehicle")
        .Produces<VehicleDto>()
        .Produces(400)
        .Produces(404);

        // DELETE /api/v1/vehicles/{id}
        vehicleGroup.MapDelete("/{id:int}", async (
            [FromRoute] int id,
            [FromServices] IMediator mediator) =>
        {
            var command = new DeleteVehicleCommand(id);
            var result = await mediator.Send(command);

            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("DeleteVehicle")
        .WithSummary("Delete a vehicle")
        .Produces(204)
        .Produces(400)
        .Produces(404);

        // GET /api/v1/vehicles/driver/{driverId}
        vehicleGroup.MapGet("/driver/{driverId:int}", async (
            [FromRoute] int driverId,
            [FromServices] IMediator mediator,
            [FromQuery] bool includeInactive = false) =>
        {
            var query = new GetVehiclesByDriverQuery(driverId)
            {
                IncludeInactive = includeInactive
            };

            var result = await mediator.Send(query);
            
            return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("GetVehiclesByDriver")
        .WithSummary("Get vehicles assigned to a specific driver")
        .Produces<IReadOnlyList<VehicleDto>>();

        // GET /api/v1/vehicles/maintenance-needed
        vehicleGroup.MapGet("/maintenance-needed", async (
            [FromServices] IMediator mediator,
            [FromQuery] int daysAhead = 30,
            [FromQuery] bool includeOverdue = true) =>
        {
            var query = new GetVehiclesNeedingMaintenanceQuery
            {
                DaysAhead = daysAhead,
                IncludeOverdue = includeOverdue
            };

            var result = await mediator.Send(query);
            
            return result.IsSuccess ? Results.Ok(result.Data) : Results.BadRequest(result.ErrorMessage);
        })
        .WithName("GetVehiclesNeedingMaintenance")
        .WithSummary("Get vehicles that need maintenance")
        .Produces<IReadOnlyList<VehicleDto>>();
    }
}
