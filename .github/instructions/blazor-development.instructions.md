---
applyTo: "**/*.razor,**/*.razor.cs"
---
# Instructions Blazor Server - FleetSyncManager

Appliquer les [directives générales de codage](./general-coding.instructions.md) et les [standards C#](./csharp-development.instructions.md) à tous les composants Blazor.

## Standards Blazor Server

### Structure des Composants
```razor
@* /Pages/Vehicles/VehiclesList.razor *@
@page "/vehicles"
@attribute [Authorize(Roles = "Administrator,FleetManager")]
@inject IVehicleService VehicleService
@inject IJSRuntime JSRuntime
@inject IMapper Mapper

<PageTitle>Gestion des Véhicules</PageTitle>

<div class="container-fluid">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="h3 mb-0">
            <i class="fas fa-car me-2"></i>
            Gestion des Véhicules
        </h1>
        <NavLink class="btn btn-primary" href="/vehicles/create">
            <i class="fas fa-plus me-1"></i>
            Nouveau Véhicule
        </NavLink>
    </div>
    
    @* Filtres *@
    <VehicleFilters @bind-Filter="currentFilter" 
                   OnFilterChanged="OnFilterChangedAsync" />
    
    @* Liste des véhicules *@
    @if (isLoading)
    {
        <LoadingSpinner />
    }
    else if (vehicles?.Items?.Any() == true)
    {
        <VehicleDataGrid Vehicles="vehicles.Items"
                        OnVehicleSelected="OnVehicleSelectedAsync"
                        OnVehicleDeleted="OnVehicleDeletedAsync" />
                        
        <Pagination CurrentPage="vehicles.Page"
                   TotalPages="vehicles.TotalPages"
                   OnPageChanged="OnPageChangedAsync" />
    }
    else
    {
        <EmptyState Message="Aucun véhicule trouvé"
                   ActionText="Ajouter le premier véhicule"
                   ActionUrl="/vehicles/create" />
    }
</div>

@code {
    private PagedResult<VehicleDto>? vehicles;
    private VehicleFilterDto currentFilter = new();
    private bool isLoading = true;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadVehiclesAsync();
    }
    
    private async Task LoadVehiclesAsync()
    {
        try
        {
            isLoading = true;
            vehicles = await VehicleService.GetVehiclesAsync(currentFilter);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync("Erreur lors du chargement des véhicules", ex.Message);
        }
        finally
        {
            isLoading = false;
        }
    }
    
    private async Task OnFilterChangedAsync(VehicleFilterDto filter)
    {
        currentFilter = filter;
        currentFilter.Page = 1; // Reset to first page
        await LoadVehiclesAsync();
    }
    
    private async Task OnPageChangedAsync(int page)
    {
        currentFilter.Page = page;
        await LoadVehiclesAsync();
    }
    
    private async Task OnVehicleSelectedAsync(VehicleDto vehicle)
    {
        // Navigation vers les détails
        Navigation.NavigateTo($"/vehicles/{vehicle.Id}");
    }
    
    private async Task OnVehicleDeletedAsync(int vehicleId)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
            "Êtes-vous sûr de vouloir supprimer ce véhicule ?");
            
        if (confirmed)
        {
            try
            {
                await VehicleService.DeleteVehicleAsync(vehicleId);
                await ShowSuccessAsync("Véhicule supprimé avec succès");
                await LoadVehiclesAsync();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Erreur lors de la suppression", ex.Message);
            }
        }
    }
    
    private async Task ShowSuccessAsync(string message)
    {
        await JSRuntime.InvokeVoidAsync("showToast", "success", message);
    }
    
    private async Task ShowErrorAsync(string title, string message)
    {
        await JSRuntime.InvokeVoidAsync("showToast", "error", $"{title}: {message}");
    }
}
```

### Composants Réutilisables
```razor
@* /Components/Common/LoadingSpinner.razor *@
<div class="d-flex justify-content-center align-items-center p-4">
    <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Chargement...</span>
    </div>
    <span class="ms-3 text-muted">@Message</span>
</div>

@code {
    [Parameter] public string Message { get; set; } = "Chargement en cours...";
}
```

```razor
@* /Components/Common/Pagination.razor *@
@if (TotalPages > 1)
{
    <nav aria-label="Navigation pagination">
        <ul class="pagination justify-content-center">
            <li class="page-item @(CurrentPage == 1 ? "disabled" : "")">
                <button class="page-link" @onclick="() => OnPageClick(CurrentPage - 1)" disabled="@(CurrentPage == 1)">
                    <i class="fas fa-chevron-left"></i>
                </button>
            </li>
            
            @foreach (var page in GetVisiblePages())
            {
                <li class="page-item @(page == CurrentPage ? "active" : "")">
                    <button class="page-link" @onclick="() => OnPageClick(page)">
                        @page
                    </button>
                </li>
            }
            
            <li class="page-item @(CurrentPage == TotalPages ? "disabled" : "")">
                <button class="page-link" @onclick="() => OnPageClick(CurrentPage + 1)" disabled="@(CurrentPage == TotalPages)">
                    <i class="fas fa-chevron-right"></i>
                </button>
            </li>
        </ul>
    </nav>
}

@code {
    [Parameter] public int CurrentPage { get; set; }
    [Parameter] public int TotalPages { get; set; }
    [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    
    private async Task OnPageClick(int page)
    {
        if (page >= 1 && page <= TotalPages && page != CurrentPage)
        {
            await OnPageChanged.InvokeAsync(page);
        }
    }
    
    private IEnumerable<int> GetVisiblePages()
    {
        const int maxVisible = 5;
        var start = Math.Max(1, CurrentPage - maxVisible / 2);
        var end = Math.Min(TotalPages, start + maxVisible - 1);
        
        if (end - start < maxVisible - 1)
        {
            start = Math.Max(1, end - maxVisible + 1);
        }
        
        return Enumerable.Range(start, end - start + 1);
    }
}
```

### Formulaires et Validation
```razor
@* /Components/Forms/VehicleForm.razor *@
<EditForm Model="Vehicle" OnValidSubmit="OnValidSubmitAsync">
    <DataAnnotationsValidator />
    <ValidationSummary class="alert alert-danger" />
    
    <div class="row">
        <div class="col-md-6">
            <div class="mb-3">
                <label for="licensePlate" class="form-label">Immatriculation *</label>
                <InputText id="licensePlate" 
                          @bind-Value="Vehicle.LicensePlate" 
                          class="form-control" 
                          placeholder="AB-123-CD" />
                <ValidationMessage For="@(() => Vehicle.LicensePlate)" class="text-danger" />
            </div>
        </div>
        
        <div class="col-md-6">
            <div class="mb-3">
                <label for="vin" class="form-label">Numéro de châssis</label>
                <InputText id="vin" 
                          @bind-Value="Vehicle.Vin" 
                          class="form-control"
                          maxlength="17" />
                <ValidationMessage For="@(() => Vehicle.Vin)" class="text-danger" />
            </div>
        </div>
    </div>
    
    <div class="row">
        <div class="col-md-4">
            <div class="mb-3">
                <label for="brand" class="form-label">Marque *</label>
                <InputText id="brand" 
                          @bind-Value="Vehicle.Brand" 
                          class="form-control" />
                <ValidationMessage For="@(() => Vehicle.Brand)" class="text-danger" />
            </div>
        </div>
        
        <div class="col-md-4">
            <div class="mb-3">
                <label for="model" class="form-label">Modèle *</label>
                <InputText id="model" 
                          @bind-Value="Vehicle.Model" 
                          class="form-control" />
                <ValidationMessage For="@(() => Vehicle.Model)" class="text-danger" />
            </div>
        </div>
        
        <div class="col-md-4">
            <div class="mb-3">
                <label for="year" class="form-label">Année *</label>
                <InputNumber id="year" 
                           @bind-Value="Vehicle.Year" 
                           class="form-control"
                           min="1950"
                           max="@(DateTime.Now.Year + 2)" />
                <ValidationMessage For="@(() => Vehicle.Year)" class="text-danger" />
            </div>
        </div>
    </div>
    
    <div class="row">
        <div class="col-md-6">
            <div class="mb-3">
                <label for="fuelType" class="form-label">Type de carburant</label>
                <InputSelect id="fuelType" 
                           @bind-Value="Vehicle.FuelType" 
                           class="form-select">
                    <option value="">-- Sélectionner --</option>
                    @foreach (var fuelType in Enum.GetValues<FuelType>())
                    {
                        <option value="@fuelType">@GetFuelTypeDisplayName(fuelType)</option>
                    }
                </InputSelect>
                <ValidationMessage For="@(() => Vehicle.FuelType)" class="text-danger" />
            </div>
        </div>
        
        <div class="col-md-6">
            <div class="mb-3">
                <label for="currentMileage" class="form-label">Kilométrage actuel</label>
                <InputNumber id="currentMileage" 
                           @bind-Value="Vehicle.CurrentMileage" 
                           class="form-control"
                           min="0" />
                <ValidationMessage For="@(() => Vehicle.CurrentMileage)" class="text-danger" />
            </div>
        </div>
    </div>
    
    <div class="d-flex justify-content-end gap-2">
        <button type="button" class="btn btn-secondary" @onclick="OnCancel">
            <i class="fas fa-times me-1"></i>
            Annuler
        </button>
        <button type="submit" class="btn btn-primary" disabled="@isSubmitting">
            @if (isSubmitting)
            {
                <div class="spinner-border spinner-border-sm me-2" role="status"></div>
            }
            else
            {
                <i class="fas fa-save me-1"></i>
            }
            @SubmitText
        </button>
    </div>
</EditForm>

@code {
    [Parameter] public CreateVehicleDto Vehicle { get; set; } = new();
    [Parameter] public string SubmitText { get; set; } = "Enregistrer";
    [Parameter] public EventCallback<CreateVehicleDto> OnValidSubmit { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    
    private bool isSubmitting = false;
    
    private async Task OnValidSubmitAsync()
    {
        isSubmitting = true;
        try
        {
            await OnValidSubmit.InvokeAsync(Vehicle);
        }
        finally
        {
            isSubmitting = false;
        }
    }
    
    private string GetFuelTypeDisplayName(FuelType fuelType)
    {
        return fuelType switch
        {
            FuelType.Gasoline => "Essence",
            FuelType.Diesel => "Diesel",
            FuelType.Electric => "Électrique",
            FuelType.Hybrid => "Hybride",
            FuelType.LPG => "GPL",
            FuelType.CNG => "GNV",
            _ => fuelType.ToString()
        };
    }
}
```

### Code-Behind Pattern
```csharp
// /Pages/Vehicles/VehiclesList.razor.cs
public partial class VehiclesList : ComponentBase, IDisposable
{
    [Inject] private IVehicleService VehicleService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IToastService ToastService { get; set; } = default!;
    
    private PagedResult<VehicleDto>? vehicles;
    private VehicleFilterDto currentFilter = new();
    private bool isLoading = true;
    private CancellationTokenSource? cancellationTokenSource;
    
    protected override async Task OnInitializedAsync()
    {
        cancellationTokenSource = new CancellationTokenSource();
        await LoadVehiclesAsync();
    }
    
    private async Task LoadVehiclesAsync()
    {
        if (cancellationTokenSource?.Token.IsCancellationRequested == true)
            return;
            
        try
        {
            isLoading = true;
            StateHasChanged();
            
            vehicles = await VehicleService.GetVehiclesAsync(currentFilter);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
        catch (Exception ex)
        {
            ToastService.ShowError("Erreur lors du chargement des véhicules", ex.Message);
        }
        finally
        {
            isLoading = false;
            if (!cancellationTokenSource?.Token.IsCancellationRequested == true)
                StateHasChanged();
        }
    }
    
    public void Dispose()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }
}
```

### Services Blazor
```csharp
// Service de Toast Notifications
public interface IToastService
{
    void ShowSuccess(string message, string? title = null);
    void ShowError(string message, string? title = null);
    void ShowWarning(string message, string? title = null);
    void ShowInfo(string message, string? title = null);
}

public class ToastService : IToastService
{
    private readonly IJSRuntime _jsRuntime;
    
    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public void ShowSuccess(string message, string? title = null)
    {
        _ = Task.Run(async () => await _jsRuntime.InvokeVoidAsync("showToast", "success", message, title));
    }
    
    public void ShowError(string message, string? title = null)
    {
        _ = Task.Run(async () => await _jsRuntime.InvokeVoidAsync("showToast", "error", message, title));
    }
    
    public void ShowWarning(string message, string? title = null)
    {
        _ = Task.Run(async () => await _jsRuntime.InvokeVoidAsync("showToast", "warning", message, title));
    }
    
    public void ShowInfo(string message, string? title = null)
    {
        _ = Task.Run(async () => await _jsRuntime.InvokeVoidAsync("showToast", "info", message, title));
    }
}
```

### Gestion des États et Performance
```razor
@* Optimisation avec ShouldRender *@
@code {
    private bool shouldRender = true;
    
    protected override bool ShouldRender()
    {
        return shouldRender;
    }
    
    private async Task OptimizedOperation()
    {
        shouldRender = false; // Prevent unnecessary renders
        
        // Long operation
        await SomeTimeConsumingOperation();
        
        shouldRender = true;
        StateHasChanged(); // Force render when needed
    }
}
```

### JavaScript Interop
```javascript
// wwwroot/js/site.js
window.showToast = (type, message, title) => {
    // Implementation avec une librarie de toast (ex: Toastr)
    toastr[type](message, title || '');
};

window.confirmDialog = (message) => {
    return confirm(message);
};

window.initializeDataTable = (tableId) => {
    // Initialisation DataTables ou autre composant JS
    $(`#${tableId}`).DataTable({
        responsive: true,
        language: {
            url: '/js/datatables-fr.json'
        }
    });
};
```

### Layout et Navigation
```razor
@* /Shared/MainLayout.razor *@
@inherits LayoutView
@inject IJSRuntime JSRuntime
@inject AuthenticationStateProvider AuthenticationStateProvider

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>
    
    <main>
        <div class="top-row px-4 auth">
            <LoginDisplay />
        </div>
        
        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<div id="blazor-error-ui">
    Environment.IsDevelopment() ? "An error has occurred. This application may no longer respond until reloaded." : "An error has occurred."
    <a href="" class="reload">Reload</a>
    <a class="dismiss">🗙</a>
</div>
```

### Bonnes Pratiques Blazor

#### Performance
- **@key :** Utiliser pour optimiser le rendu des listes
- **ShouldRender() :** Contrôler le rendu des composants
- **Virtualization :** Pour les grandes listes
- **Lazy Loading :** Chargement différé des composants

#### Sécurité
- **[Authorize] :** Protéger les pages sensibles
- **Validation côté serveur :** Toujours valider sur le serveur
- **Sanitization HTML :** Nettoyer les entrées HTML

#### UX/UI
- **Loading states :** Afficher l'état de chargement
- **Error handling :** Gestion gracieuse des erreurs
- **Responsive design :** Interface adaptative
- **Accessibility :** Respect des standards WCAG

#### Architecture
- **Séparation des préoccupations :** Code-behind pour logique complexe
- **Composants réutilisables :** Maximiser la réutilisabilité
- **Services métier :** Déléguer la logique aux services
- **State management :** Utiliser des services pour l'état partagé

Ces instructions garantissent un développement Blazor cohérent et performant pour FleetSyncManager.
