# Script PowerShell pour initialiser Git et sauvegarder FleetSyncManager
# Exécuter depuis le répertoire racine du projet

Write-Host "🚀 Initialisation du dépôt Git pour FleetSyncManager..." -ForegroundColor Cyan

# Vérifier si nous sommes dans le bon répertoire
$currentPath = Get-Location
Write-Host "📍 Répertoire actuel: $currentPath" -ForegroundColor Yellow

# Initialiser le dépôt Git si nécessaire
if (-not (Test-Path ".git")) {
    Write-Host "📦 Initialisation du dépôt Git..." -ForegroundColor Green
    git init
    
    # Configurer Git (optionnel - remplacer par vos informations)
    Write-Host "⚙️ Configuration Git..." -ForegroundColor Green
    git config user.name "FleetSyncManager Developer"
    git config user.email "developer@fleetsynmanager.com"
    
    # Configurer la branche principale
    git branch -M main
} else {
    Write-Host "✅ Dépôt Git déjà initialisé" -ForegroundColor Green
}

# Ajouter tous les fichiers
Write-Host "📁 Ajout des fichiers au staging..." -ForegroundColor Green
git add .

# Vérifier le statut
Write-Host "📋 Statut Git:" -ForegroundColor Yellow
git status --short

# Faire le commit initial
Write-Host "💾 Création du commit initial..." -ForegroundColor Green
$commitMessage = "🎉 Initial commit - FleetSyncManager v1.0

✨ Fonctionnalités implémentées:
- 🏗️ Clean Architecture avec .NET 9.0
- 🔧 CQRS avec MediatR
- 🌐 Blazor Server UI
- 📡 Minimal API REST
- ✅ FluentValidation
- 📊 Entités métier complètes (Vehicle, Driver, Maintenance, Incident)
- 🎨 Interface responsive Bootstrap 5.3
- 📝 Documentation technique complète

🛠️ Technologies:
- .NET 9.0 / ASP.NET Core
- Blazor Server
- MediatR
- Entity Framework Core
- PostgreSQL
- AutoMapper
- Serilog
- Bootstrap 5.3 + Font Awesome

📋 Architecture:
- Domain Layer (Entities, Enums, Interfaces)
- Application Layer (Commands, Queries, DTOs, Validators)
- Infrastructure Layer (Data, Repositories, Services)
- Web Layer (Blazor Components, API Endpoints)

🎯 Prêt pour le développement des 4 sprints planifiés!"

git commit -m "$commitMessage"

# Afficher le résumé
Write-Host "📊 Résumé du commit:" -ForegroundColor Cyan
git log --oneline -1
git show --stat HEAD

Write-Host "`n🎉 Sauvegarde Git terminée avec succès!" -ForegroundColor Green
Write-Host "📂 Dépôt Git local créé dans: $currentPath" -ForegroundColor Yellow

# Afficher les prochaines étapes
Write-Host "`n🚀 Prochaines étapes:" -ForegroundColor Cyan
Write-Host "1. Configurer un dépôt distant (GitHub, Azure DevOps, etc.)" -ForegroundColor White
Write-Host "2. git remote add origin <url-du-depot-distant>" -ForegroundColor Gray
Write-Host "3. git push -u origin main" -ForegroundColor Gray
Write-Host "`n4. Continuer le développement selon le backlog MVP" -ForegroundColor White
Write-Host "5. Créer des branches pour chaque feature/sprint" -ForegroundColor Gray

Write-Host "`n💡 Structure sauvegardée:" -ForegroundColor Cyan
Write-Host "✅ Solution .NET 9.0 complète" -ForegroundColor Green
Write-Host "✅ 4 projets Clean Architecture" -ForegroundColor Green
Write-Host "✅ Blazor Server + Minimal API" -ForegroundColor Green
Write-Host "✅ Documentation technique" -ForegroundColor Green
Write-Host "✅ Instructions de développement" -ForegroundColor Green
