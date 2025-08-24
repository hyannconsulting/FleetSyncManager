using Laroche.FleetManager.Application;

var builder = WebApplication.CreateBuilder(args);

// Ajout des services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configuration MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
});

// Configuration des services d'application
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configuration des middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configuration Blazor
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
