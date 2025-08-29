-- Script d'initialisation pour FleetManager PostgreSQL
-- Ce script est exécuté automatiquement lors du premier démarrage du conteneur

-- Création d'extensions utiles
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Commentaires de base de données
COMMENT ON DATABASE fleetmanager_dev IS 'Base de données de développement pour FleetSyncManager';

-- Configuration initiale
SET timezone = 'UTC';

-- Création d'un utilisateur pour l'application si nécessaire
-- (l'utilisateur principal est déjà créé par les variables d'environnement)

-- Log de fin d'initialisation
SELECT 'Initialisation PostgreSQL terminée pour FleetManager' as status;
