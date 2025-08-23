# Script de vérification et finalisation Git

Write-Host "🔍 Vérification de l'état Git..." -ForegroundColor Cyan

# Vérifier si nous sommes dans un dépôt Git
if (Test-Path ".git") {
    Write-Host "✅ Dépôt Git détecté" -ForegroundColor Green
    
    # Afficher les informations du dépôt
    Write-Host "`n📊 Informations du dépôt:" -ForegroundColor Yellow
    git branch
    git status --porcelain | Measure-Object | ForEach-Object { Write-Host "📁 Fichiers trackés: $($_.Count)" }
    
} else {
    Write-Host "❌ Aucun dépôt Git trouvé" -ForegroundColor Red
    Write-Host "Initialisation manuelle requise..." -ForegroundColor Yellow
}

# Lister les fichiers dans le répertoire pour vérification
Write-Host "`n📂 Contenu du répertoire:" -ForegroundColor Yellow
Get-ChildItem -Recurse -Directory | Select-Object Name, FullName | Format-Table -AutoSize
