-- =============================================================================
-- FleetSyncManager Database Schema - PostgreSQL
-- =============================================================================
-- Généré automatiquement pour Entity Framework Core
-- Version: 1.0.0
-- Date: 2025-08-28
-- =============================================================================

-- Extensions PostgreSQL nécessaires
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- =============================================================================
-- Tables ASP.NET Core Identity
-- =============================================================================

-- Table des rôles
CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL,
    "Name" CHARACTER VARYING(256),
    "NormalizedName" CHARACTER VARYING(256),
    "ConcurrencyStamp" TEXT,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

-- Table des utilisateurs
CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL,
    "UserName" CHARACTER VARYING(256),
    "NormalizedUserName" CHARACTER VARYING(256),
    "Email" CHARACTER VARYING(256),
    "NormalizedEmail" CHARACTER VARYING(256),
    "EmailConfirmed" BOOLEAN NOT NULL,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL,
    "TwoFactorEnabled" BOOLEAN NOT NULL,
    "LockoutEnd" TIMESTAMP WITH TIME ZONE,
    "LockoutEnabled" BOOLEAN NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

-- Table des claims de rôles
CREATE TABLE "AspNetRoleClaims" (
    "Id" SERIAL NOT NULL,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

-- Table des claims d'utilisateurs
CREATE TABLE "AspNetUserClaims" (
    "Id" SERIAL NOT NULL,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Table des logins d'utilisateurs
CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Table des rôles d'utilisateurs
CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- Table des tokens d'utilisateurs
CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

-- =============================================================================
-- Tables Domaine FleetSync
-- =============================================================================

-- Table des conducteurs
CREATE TABLE "Drivers" (
    "Id" SERIAL NOT NULL,
    "FirstName" CHARACTER VARYING(100) NOT NULL,
    "LastName" CHARACTER VARYING(100) NOT NULL,
    "Email" CHARACTER VARYING(255) NOT NULL,
    "PhoneNumber" CHARACTER VARYING(20),
    "LicenseNumber" CHARACTER VARYING(50) NOT NULL,
    "LicenseType" INTEGER NOT NULL, -- 0=B, 1=C, 2=D, etc.
    "LicenseExpiryDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "HireDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1, -- 0=Inactive, 1=Active, 2=Suspended
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    CONSTRAINT "PK_Drivers" PRIMARY KEY ("Id"),
    CONSTRAINT "UK_Drivers_Email" UNIQUE ("Email"),
    CONSTRAINT "UK_Drivers_LicenseNumber" UNIQUE ("LicenseNumber")
);

-- Table des véhicules
CREATE TABLE "Vehicles" (
    "Id" SERIAL NOT NULL,
    "LicensePlate" CHARACTER VARYING(20) NOT NULL,
    "Vin" CHARACTER VARYING(17),
    "Brand" CHARACTER VARYING(50) NOT NULL,
    "Model" CHARACTER VARYING(50) NOT NULL,
    "Year" INTEGER NOT NULL,
    "FuelType" INTEGER NOT NULL, -- 0=Gasoline, 1=Diesel, 2=Electric, etc.
    "CurrentMileage" INTEGER NOT NULL DEFAULT 0,
    "Status" INTEGER NOT NULL DEFAULT 1, -- 0=Inactive, 1=Active, 2=Maintenance, 3=Decommissioned
    "PurchaseDate" TIMESTAMP WITH TIME ZONE,
    "PurchasePrice" NUMERIC(12,2),
    "InsurancePolicyNumber" CHARACTER VARYING(100),
    "InsuranceExpiryDate" TIMESTAMP WITH TIME ZONE,
    "NextMaintenanceDue" TIMESTAMP WITH TIME ZONE,
    "NextMaintenanceMileage" INTEGER,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    CONSTRAINT "PK_Vehicles" PRIMARY KEY ("Id"),
    CONSTRAINT "UK_Vehicles_LicensePlate" UNIQUE ("LicensePlate"),
    CONSTRAINT "CK_Vehicles_Year" CHECK ("Year" >= 1900 AND "Year" <= 2100),
    CONSTRAINT "CK_Vehicles_CurrentMileage" CHECK ("CurrentMileage" >= 0)
);

-- Table des assignations véhicule-conducteur
CREATE TABLE "VehicleAssignments" (
    "Id" SERIAL NOT NULL,
    "VehicleId" INTEGER NOT NULL,
    "DriverId" INTEGER NOT NULL,
    "AssignedDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UnassignedDate" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    CONSTRAINT "PK_VehicleAssignments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_VehicleAssignments_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_VehicleAssignments_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers" ("Id") ON DELETE CASCADE
);

-- Table des enregistrements de maintenance
CREATE TABLE "MaintenanceRecords" (
    "Id" SERIAL NOT NULL,
    "VehicleId" INTEGER NOT NULL,
    "MaintenanceType" INTEGER NOT NULL, -- 0=Preventive, 1=Corrective, 2=Emergency
    "Description" TEXT NOT NULL,
    "MaintenanceDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "MileageAtMaintenance" INTEGER NOT NULL,
    "Cost" NUMERIC(10,2),
    "ServiceProvider" CHARACTER VARYING(200),
    "NextMaintenanceDue" TIMESTAMP WITH TIME ZONE,
    "NextMaintenanceMileage" INTEGER,
    "Status" INTEGER NOT NULL DEFAULT 0, -- 0=Scheduled, 1=InProgress, 2=Completed, 3=Cancelled
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    CONSTRAINT "PK_MaintenanceRecords" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MaintenanceRecords_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "CK_MaintenanceRecords_Cost" CHECK ("Cost" >= 0)
);

-- Table des incidents
CREATE TABLE "Incidents" (
    "Id" SERIAL NOT NULL,
    "VehicleId" INTEGER NOT NULL,
    "DriverId" INTEGER,
    "IncidentType" INTEGER NOT NULL, -- 0=Accident, 1=Breakdown, 2=Theft, 3=Damage
    "Severity" INTEGER NOT NULL, -- 0=Minor, 1=Moderate, 2=Major, 3=Critical
    "Description" TEXT NOT NULL,
    "IncidentDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Location" CHARACTER VARYING(500),
    "PoliceReportNumber" CHARACTER VARYING(100),
    "InsuranceClaimNumber" CHARACTER VARYING(100),
    "EstimatedCost" NUMERIC(12,2),
    "ActualCost" NUMERIC(12,2),
    "Status" INTEGER NOT NULL DEFAULT 0, -- 0=Reported, 1=UnderInvestigation, 2=Resolved, 3=Closed
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    CONSTRAINT "PK_Incidents" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Incidents_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Incidents_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers" ("Id") ON DELETE SET NULL
);

-- Table des enregistrements de suivi GPS
CREATE TABLE "GpsTrackingRecords" (
    "Id" SERIAL NOT NULL,
    "VehicleId" INTEGER NOT NULL,
    "Latitude" NUMERIC(10,8) NOT NULL,
    "Longitude" NUMERIC(11,8) NOT NULL,
    "Altitude" NUMERIC(8,2),
    "Speed" NUMERIC(6,2),
    "Heading" NUMERIC(5,2),
    "RecordedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "DeviceId" CHARACTER VARYING(100),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    CONSTRAINT "PK_GpsTrackingRecords" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_GpsTrackingRecords_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "CK_GpsTrackingRecords_Latitude" CHECK ("Latitude" >= -90 AND "Latitude" <= 90),
    CONSTRAINT "CK_GpsTrackingRecords_Longitude" CHECK ("Longitude" >= -180 AND "Longitude" <= 180)
);

-- =============================================================================
-- Index pour les performances
-- =============================================================================

-- Index Identity
CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");
CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");
CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");
CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");
CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");
CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

-- Index Domaine
CREATE INDEX "IX_Drivers_Email" ON "Drivers" ("Email");
CREATE INDEX "IX_Drivers_Status" ON "Drivers" ("Status");
CREATE INDEX "IX_Drivers_LicenseExpiryDate" ON "Drivers" ("LicenseExpiryDate");

CREATE INDEX "IX_Vehicles_LicensePlate" ON "Vehicles" ("LicensePlate");
CREATE INDEX "IX_Vehicles_Status" ON "Vehicles" ("Status");
CREATE INDEX "IX_Vehicles_Brand" ON "Vehicles" ("Brand");
CREATE INDEX "IX_Vehicles_NextMaintenanceDue" ON "Vehicles" ("NextMaintenanceDue");
CREATE INDEX "IX_Vehicles_InsuranceExpiryDate" ON "Vehicles" ("InsuranceExpiryDate");

CREATE INDEX "IX_VehicleAssignments_VehicleId" ON "VehicleAssignments" ("VehicleId");
CREATE INDEX "IX_VehicleAssignments_DriverId" ON "VehicleAssignments" ("DriverId");
CREATE INDEX "IX_VehicleAssignments_IsActive" ON "VehicleAssignments" ("IsActive");
CREATE INDEX "IX_VehicleAssignments_AssignedDate" ON "VehicleAssignments" ("AssignedDate");

CREATE INDEX "IX_MaintenanceRecords_VehicleId" ON "MaintenanceRecords" ("VehicleId");
CREATE INDEX "IX_MaintenanceRecords_MaintenanceDate" ON "MaintenanceRecords" ("MaintenanceDate");
CREATE INDEX "IX_MaintenanceRecords_Status" ON "MaintenanceRecords" ("Status");
CREATE INDEX "IX_MaintenanceRecords_NextMaintenanceDue" ON "MaintenanceRecords" ("NextMaintenanceDue");

CREATE INDEX "IX_Incidents_VehicleId" ON "Incidents" ("VehicleId");
CREATE INDEX "IX_Incidents_DriverId" ON "Incidents" ("DriverId");
CREATE INDEX "IX_Incidents_IncidentDate" ON "Incidents" ("IncidentDate");
CREATE INDEX "IX_Incidents_Status" ON "Incidents" ("Status");

CREATE INDEX "IX_GpsTrackingRecords_VehicleId" ON "GpsTrackingRecords" ("VehicleId");
CREATE INDEX "IX_GpsTrackingRecords_RecordedAt" ON "GpsTrackingRecords" ("RecordedAt");
CREATE INDEX "IX_GpsTrackingRecords_VehicleId_RecordedAt" ON "GpsTrackingRecords" ("VehicleId", "RecordedAt");

-- Index géographique pour le GPS (optionnel si PostGIS est disponible)
-- CREATE INDEX "IX_GpsTrackingRecords_Location" ON "GpsTrackingRecords" USING gist (ll_to_earth("Latitude", "Longitude"));

-- =============================================================================
-- Contraintes supplémentaires et règles métier
-- =============================================================================

-- Un seul conducteur actif par véhicule à la fois
CREATE UNIQUE INDEX "IX_VehicleAssignments_VehicleId_Active" 
    ON "VehicleAssignments" ("VehicleId") 
    WHERE "IsActive" = TRUE;

-- Un conducteur ne peut être assigné qu'à un seul véhicule à la fois
CREATE UNIQUE INDEX "IX_VehicleAssignments_DriverId_Active" 
    ON "VehicleAssignments" ("DriverId") 
    WHERE "IsActive" = TRUE;

-- =============================================================================
-- Vues utilitaires
-- =============================================================================

-- Vue des véhicules avec leur conducteur actuel
CREATE VIEW "VehiclesWithCurrentDriver" AS
SELECT 
    v."Id" as "VehicleId",
    v."LicensePlate",
    v."Brand",
    v."Model",
    v."Year",
    v."Status" as "VehicleStatus",
    d."Id" as "DriverId",
    d."FirstName",
    d."LastName",
    d."Email",
    va."AssignedDate"
FROM "Vehicles" v
LEFT JOIN "VehicleAssignments" va ON v."Id" = va."VehicleId" AND va."IsActive" = TRUE
LEFT JOIN "Drivers" d ON va."DriverId" = d."Id";

-- Vue des maintenances dues
CREATE VIEW "MaintenanceDue" AS
SELECT 
    v."Id" as "VehicleId",
    v."LicensePlate",
    v."Brand",
    v."Model",
    v."NextMaintenanceDue",
    v."NextMaintenanceMileage",
    v."CurrentMileage",
    CASE 
        WHEN v."NextMaintenanceDue" <= CURRENT_DATE THEN 'DATE_DUE'
        WHEN v."NextMaintenanceMileage" IS NOT NULL AND v."CurrentMileage" >= v."NextMaintenanceMileage" THEN 'MILEAGE_DUE'
        ELSE 'NOT_DUE'
    END as "MaintenanceStatus"
FROM "Vehicles" v
WHERE v."Status" = 1 -- Active vehicles only
    AND (v."NextMaintenanceDue" <= CURRENT_DATE + INTERVAL '30 days' 
         OR (v."NextMaintenanceMileage" IS NOT NULL AND v."CurrentMileage" >= v."NextMaintenanceMileage" - 1000));

-- =============================================================================
-- Données de test (optionnel)
-- =============================================================================

-- Insertion de rôles par défaut
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES 
    (uuid_generate_v4()::TEXT, 'Admin', 'ADMIN', uuid_generate_v4()::TEXT),
    (uuid_generate_v4()::TEXT, 'Manager', 'MANAGER', uuid_generate_v4()::TEXT),
    (uuid_generate_v4()::TEXT, 'User', 'USER', uuid_generate_v4()::TEXT);

-- =============================================================================
-- Fin du script
-- =============================================================================

-- Mise à jour des statistiques PostgreSQL
ANALYZE;

-- Affichage du résumé
DO $$
BEGIN
    RAISE NOTICE '=============================================================================';
    RAISE NOTICE 'FleetSyncManager Database Schema créé avec succès!';
    RAISE NOTICE 'Tables créées: %, Index créés: %, Vues créées: %', 
        (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public'),
        (SELECT COUNT(*) FROM pg_indexes WHERE schemaname = 'public'),
        (SELECT COUNT(*) FROM information_schema.views WHERE table_schema = 'public');
    RAISE NOTICE '=============================================================================';
END
$$;
