@echo off
chcp 65001 >nul
title FleetSyncManager - Démarrage

echo.
echo 🚀 Démarrage de FleetSyncManager
echo =================================
echo.

set API_PATH=%~dp0src\Laroche.FleetManager.API
set WEB_PATH=%~dp0src\Laroche.FleetManager.Web

echo 📁 Vérification des chemins...
if not exist "%API_PATH%" (
    echo ❌ Chemin API non trouvé: %API_PATH%
    pause
    exit /b 1
)

if not exist "%WEB_PATH%" (
    echo ❌ Chemin Web non trouvé: %WEB_PATH%
    pause
    exit /b 1
)

echo ✅ Chemins vérifiés
echo.

echo 🌐 Lancement de l'API (Port 7002)...
cd /d "%API_PATH%"
start "FleetSyncManager API" cmd /c "dotnet run --urls https://localhost:7002"

echo ⏳ Attente de 5 secondes...
timeout /t 5 /nobreak >nul

echo.
echo 🎨 Lancement de l'interface Web (Port 7001)...
cd /d "%WEB_PATH%"
start "FleetSyncManager Web" cmd /c "dotnet run --urls https://localhost:7001"

echo.
echo ✅ Applications lancées avec succès!
echo.
echo 🌍 URLs d'accès:
echo    📡 API: https://localhost:7002
echo    📡 API Docs: https://localhost:7002/swagger
echo    🎨 Web: https://localhost:7001
echo.
echo 💡 Conseils:
echo    - L'API doit être accessible avant d'utiliser le Web
echo    - Vérifiez les logs en cas de problème
echo    - Fermez les fenêtres cmd pour arrêter les services
echo.
echo ⏳ Attente du démarrage complet (15 secondes)...
timeout /t 15 /nobreak >nul

echo.
echo 🌐 Ouverture du navigateur...
start https://localhost:7001

echo.
echo ✨ FleetSyncManager est maintenant accessible!
echo.
pause
