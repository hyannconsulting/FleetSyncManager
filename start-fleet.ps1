# Script PowerShell pour lancer FleetSyncManager API et Web
Write-Host "🚀 Démarrage de FleetSyncManager" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

$rootPath = $PSScriptRoot
$apiPath = Join-Path $rootPath "src\Laroche.FleetManager.API"
$webPath = Join-Path $rootPath "src\Laroche.FleetManager.Web"

# Vérification des chemins
if (-not (Test-Path $apiPath)) {
    Write-Host "❌ Chemin API non trouvé: $apiPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $webPath)) {
    Write-Host "❌ Chemin Web non trouvé: $webPath" -ForegroundColor Red
    exit 1
}

Write-Host "📁 API Path: $apiPath" -ForegroundColor Cyan
Write-Host "📁 Web Path: $webPath" -ForegroundColor Cyan

# Fonction pour démarrer un processus en arrière-plan
function Start-BackgroundProcess {
    param(
        [string]$Title,
        [string]$WorkingDirectory,
        [string]$Command,
        [string]$Arguments
    )
    
    Write-Host "🔄 Démarrage de $Title..." -ForegroundColor Yellow
    
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $Command
    $psi.Arguments = $Arguments
    $psi.WorkingDirectory = $WorkingDirectory
    $psi.UseShellExecute = $true
    $psi.CreateNoWindow = $false
    $psi.WindowStyle = "Normal"
    
    $process = [System.Diagnostics.Process]::Start($psi)
    return $process
}

try {
    Write-Host ""
    Write-Host "🌐 Lancement de l'API (Port 7002)..." -ForegroundColor Magenta
    $apiProcess = Start-BackgroundProcess -Title "FleetSyncManager API" -WorkingDirectory $apiPath -Command "dotnet" -Arguments "run --urls https://localhost:7002"
    
    Start-Sleep -Seconds 3
    
    Write-Host "🎨 Lancement de l'interface Web (Port 7001)..." -ForegroundColor Magenta  
    $webProcess = Start-BackgroundProcess -Title "FleetSyncManager Web" -WorkingDirectory $webPath -Command "dotnet" -Arguments "run --urls https://localhost:7001"
    
    Write-Host ""
    Write-Host "✅ Applications lancées avec succès!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌍 URLs d'accès:" -ForegroundColor White
    Write-Host "   📡 API: https://localhost:7002" -ForegroundColor Cyan
    Write-Host "   📡 API Docs: https://localhost:7002/swagger" -ForegroundColor Cyan
    Write-Host "   🎨 Web: https://localhost:7001" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "💡 Conseils:" -ForegroundColor White
    Write-Host "   - L'API doit être accessible avant d'utiliser le Web" -ForegroundColor Gray
    Write-Host "   - Vérifiez les logs en cas de problème" -ForegroundColor Gray
    Write-Host "   - Utilisez Ctrl+C pour arrêter chaque processus" -ForegroundColor Gray
    Write-Host ""
    Write-Host "⏳ Attente du démarrage complet... (15 secondes)" -ForegroundColor Yellow
    
    Start-Sleep -Seconds 15
    
    # Tentative d'ouverture du navigateur
    Write-Host "🌐 Ouverture du navigateur..." -ForegroundColor Green
    Start-Process "https://localhost:7001"
    
    Write-Host ""
    Write-Host "✨ FleetSyncManager est maintenant accessible!" -ForegroundColor Green
    Write-Host "Appuyez sur une touche pour fermer ce script (les applications continuent de fonctionner)..."
    
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    
    Write-Host ""
    Write-Host "👋 Script fermé. Les applications continuent de fonctionner en arrière-plan." -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "❌ Erreur lors du démarrage: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Vérifiez les prérequis et les ports disponibles." -ForegroundColor Yellow
}
