# Schéma de Base de Données PostgreSQL - FleetSyncManager MVP

## Vue d'Ensemble

### Base de données : `fleetsyncmanager`
### Version PostgreSQL : 15+
### Encodage : UTF-8
### Collation : fr_FR.UTF-8

## Tables et Relations

### Diagramme ERD Conceptuel
```
Users (ASP.NET Identity)
    │
    ├─── UserRoles ────┐
    │                  │
    ▼                  ▼
Drivers ───────── Vehicles
    │                  │
    │                  ├─── VehicleDocuments
    │                  ├─── MaintenanceRecords
    │                  └─── MileageRecords
    │
    ├─── DriverDocuments
    ├─── Violations
    │
    └─── Incidents ◄───┘
            │
            ├─── IncidentDocuments
            └─── IncidentNotes
```

## Scripts de Création des Tables

### 1. Tables d'Authentification (ASP.NET Identity)
```sql
-- Tables générées automatiquement par ASP.NET Core Identity
-- AspNetUsers, AspNetRoles, AspNetUserRoles, etc.

-- Extension de la table utilisateur
CREATE TABLE "AspNetUsers" (
    "Id" VARCHAR(450) NOT NULL,
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" BOOLEAN NOT NULL,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" TEXT,
    "PhoneNumberConfirmed" BOOLEAN NOT NULL,
    "TwoFactorEnabled" BOOLEAN NOT NULL,
    "LockoutEnd" TIMESTAMPTZ,
    "LockoutEnabled" BOOLEAN NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL,
    -- Extensions personnalisées
    "FirstName" VARCHAR(50),
    "LastName" VARCHAR(50),
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ,
    
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);
```

### 2. Table Drivers (Conducteurs)
```sql
CREATE TABLE "Drivers" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(50) NOT NULL,
    "LastName" VARCHAR(50) NOT NULL,
    "DateOfBirth" DATE NOT NULL,
    "Email" VARCHAR(100) UNIQUE NOT NULL,
    "PhoneNumber" VARCHAR(20),
    "Address" TEXT,
    "PostalCode" VARCHAR(10),
    "City" VARCHAR(100),
    "PhotoUrl" VARCHAR(500),
    
    -- Contact d'urgence
    "EmergencyContactName" VARCHAR(100),
    "EmergencyContactPhone" VARCHAR(20),
    "EmergencyContactRelation" VARCHAR(50),
    
    -- Statut
    "Status" INTEGER DEFAULT 1, -- 1=Active, 2=Suspended, 3=OnLeave, 4=Terminated
    "HireDate" DATE NOT NULL,
    "TerminationDate" DATE,
    
    -- Permis de conduire
    "LicenseObtainedDate" DATE NOT NULL,
    "LicenseExpiryDate" DATE NOT NULL,
    "LicenseNumber" VARCHAR(50) UNIQUE NOT NULL,
    "LicenseTypes" VARCHAR(100), -- JSON: ["B", "C1", "D"]
    "RemainingPoints" INTEGER DEFAULT 12,
    
    -- Suivi médical
    "LastMedicalCheckDate" DATE,
    "NextMedicalCheckDate" DATE,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ,
    "CreatedBy" VARCHAR(450),
    "UpdatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_Drivers_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id"),
    CONSTRAINT "FK_Drivers_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES "AspNetUsers"("Id")
);

-- Index pour optimiser les recherches
CREATE INDEX "IX_Drivers_Email" ON "Drivers" ("Email");
CREATE INDEX "IX_Drivers_LastName_FirstName" ON "Drivers" ("LastName", "FirstName");
CREATE INDEX "IX_Drivers_Status" ON "Drivers" ("Status");
CREATE INDEX "IX_Drivers_LicenseExpiryDate" ON "Drivers" ("LicenseExpiryDate");
```

### 3. Table Vehicles (Véhicules)
```sql
CREATE TABLE "Vehicles" (
    "Id" SERIAL PRIMARY KEY,
    
    -- Identification
    "LicensePlate" VARCHAR(20) UNIQUE NOT NULL,
    "Vin" VARCHAR(17) UNIQUE,
    "Brand" VARCHAR(50) NOT NULL,
    "Model" VARCHAR(50) NOT NULL,
    "Year" INTEGER NOT NULL,
    "Color" VARCHAR(30),
    
    -- Caractéristiques techniques
    "FuelType" INTEGER NOT NULL, -- 1=Gasoline, 2=Diesel, 3=Electric, etc.
    "EngineSize" INTEGER, -- Cylindrée en cm3
    "Power" INTEGER, -- Puissance en CV
    "Category" INTEGER DEFAULT 1, -- 1=Car, 2=LightTruck, 3=HeavyTruck, etc.
    
    -- État et utilisation
    "Status" INTEGER DEFAULT 1, -- 1=Active, 2=Maintenance, 3=OutOfOrder, 4=Retired
    "CurrentMileage" INTEGER DEFAULT 0,
    "LastMileageUpdate" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    
    -- Dates importantes
    "PurchaseDate" DATE NOT NULL,
    "WarrantyEndDate" DATE,
    "NextTechnicalInspection" DATE NOT NULL,
    "InsuranceExpiryDate" DATE NOT NULL,
    "NextMaintenanceDate" DATE,
    "NextMaintenanceMileage" INTEGER,
    
    -- Coûts et valeurs
    "PurchasePrice" DECIMAL(10,2),
    "CurrentValue" DECIMAL(10,2),
    "MonthlyInsuranceCost" DECIMAL(8,2),
    
    -- Affectation
    "AssignedDriverId" INTEGER,
    "AssignedDate" DATE,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ,
    "CreatedBy" VARCHAR(450),
    "UpdatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_Vehicles_AssignedDriver" FOREIGN KEY ("AssignedDriverId") REFERENCES "Drivers"("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Vehicles_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id"),
    CONSTRAINT "FK_Vehicles_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES "AspNetUsers"("Id"),
    CONSTRAINT "CK_Vehicles_Year" CHECK ("Year" BETWEEN 1950 AND EXTRACT(YEAR FROM CURRENT_DATE) + 2),
    CONSTRAINT "CK_Vehicles_CurrentMileage" CHECK ("CurrentMileage" >= 0)
);

-- Index pour optimiser les performances
CREATE INDEX "IX_Vehicles_LicensePlate" ON "Vehicles" ("LicensePlate");
CREATE INDEX "IX_Vehicles_AssignedDriverId" ON "Vehicles" ("AssignedDriverId");
CREATE INDEX "IX_Vehicles_Status" ON "Vehicles" ("Status");
CREATE INDEX "IX_Vehicles_NextTechnicalInspection" ON "Vehicles" ("NextTechnicalInspection");
CREATE INDEX "IX_Vehicles_InsuranceExpiryDate" ON "Vehicles" ("InsuranceExpiryDate");
CREATE INDEX "IX_Vehicles_Brand_Model" ON "Vehicles" ("Brand", "Model");
```

### 4. Table VehicleDocuments (Documents Véhicules)
```sql
CREATE TABLE "VehicleDocuments" (
    "Id" SERIAL PRIMARY KEY,
    "VehicleId" INTEGER NOT NULL,
    "DocumentType" INTEGER NOT NULL, -- 1=CarteGrise, 2=Assurance, 3=Contrat, etc.
    "DocumentName" VARCHAR(200) NOT NULL,
    "FileName" VARCHAR(255) NOT NULL,
    "FilePath" VARCHAR(500) NOT NULL,
    "FileSize" BIGINT,
    "MimeType" VARCHAR(100),
    "ExpiryDate" DATE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "Description" TEXT,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_VehicleDocuments_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_VehicleDocuments_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_VehicleDocuments_VehicleId" ON "VehicleDocuments" ("VehicleId");
CREATE INDEX "IX_VehicleDocuments_DocumentType" ON "VehicleDocuments" ("DocumentType");
CREATE INDEX "IX_VehicleDocuments_ExpiryDate" ON "VehicleDocuments" ("ExpiryDate");
```

### 5. Table DriverDocuments (Documents Conducteurs)
```sql
CREATE TABLE "DriverDocuments" (
    "Id" SERIAL PRIMARY KEY,
    "DriverId" INTEGER NOT NULL,
    "DocumentType" INTEGER NOT NULL, -- 1=Permis, 2=VisiteMedicale, 3=PhotoIdentite, etc.
    "DocumentName" VARCHAR(200) NOT NULL,
    "FileName" VARCHAR(255) NOT NULL,
    "FilePath" VARCHAR(500) NOT NULL,
    "FileSize" BIGINT,
    "MimeType" VARCHAR(100),
    "ExpiryDate" DATE,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "Description" TEXT,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_DriverDocuments_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DriverDocuments_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_DriverDocuments_DriverId" ON "DriverDocuments" ("DriverId");
CREATE INDEX "IX_DriverDocuments_DocumentType" ON "DriverDocuments" ("DocumentType");
```

### 6. Table Incidents (Sinistres)
```sql
CREATE TABLE "Incidents" (
    "Id" SERIAL PRIMARY KEY,
    "IncidentNumber" VARCHAR(20) UNIQUE NOT NULL, -- Format: INC-YYYY-XXXXXX
    "Type" INTEGER NOT NULL, -- 1=Accident, 2=Theft, 3=Vandalism, etc.
    "Severity" INTEGER NOT NULL, -- 1=Minor, 2=Major, 3=Critical
    "Status" INTEGER DEFAULT 1, -- 1=Declared, 2=InProgress, 3=Closed
    
    -- Relations
    "VehicleId" INTEGER NOT NULL,
    "DriverId" INTEGER NOT NULL,
    
    -- Circonstances
    "IncidentDate" DATE NOT NULL,
    "IncidentTime" TIME NOT NULL,
    "Location" VARCHAR(500),
    "Description" TEXT NOT NULL,
    "WeatherConditions" VARCHAR(100),
    "RoadConditions" VARCHAR(100),
    
    -- Responsabilité
    "DriverAtFault" BOOLEAN DEFAULT FALSE,
    "FaultDescription" TEXT,
    
    -- Tiers impliqués
    "ThirdPartyInvolved" BOOLEAN DEFAULT FALSE,
    "ThirdPartyDetails" TEXT,
    "WitnessDetails" TEXT,
    
    -- Coûts et assurance
    "EstimatedCost" DECIMAL(10,2) DEFAULT 0,
    "ActualCost" DECIMAL(10,2) DEFAULT 0,
    "Deductible" DECIMAL(8,2) DEFAULT 0,
    "InsuranceClaimNumber" VARCHAR(50),
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ,
    "CreatedBy" VARCHAR(450),
    "UpdatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_Incidents_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id"),
    CONSTRAINT "FK_Incidents_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers"("Id"),
    CONSTRAINT "FK_Incidents_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id"),
    CONSTRAINT "FK_Incidents_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_Incidents_IncidentNumber" ON "Incidents" ("IncidentNumber");
CREATE INDEX "IX_Incidents_VehicleId" ON "Incidents" ("VehicleId");
CREATE INDEX "IX_Incidents_DriverId" ON "Incidents" ("DriverId");
CREATE INDEX "IX_Incidents_Status" ON "Incidents" ("Status");
CREATE INDEX "IX_Incidents_IncidentDate" ON "Incidents" ("IncidentDate");
```

### 7. Table MaintenanceRecords (Historique Maintenance)
```sql
CREATE TABLE "MaintenanceRecords" (
    "Id" SERIAL PRIMARY KEY,
    "VehicleId" INTEGER NOT NULL,
    "Type" INTEGER NOT NULL, -- 1=Revision, 2=Repair, 3=TechnicalInspection, etc.
    "Description" TEXT NOT NULL,
    "MaintenanceDate" DATE NOT NULL,
    "MileageAtMaintenance" INTEGER,
    "Cost" DECIMAL(10,2) DEFAULT 0,
    "Supplier" VARCHAR(200),
    "InvoiceNumber" VARCHAR(100),
    "NextMaintenanceDate" DATE,
    "NextMaintenanceMileage" INTEGER,
    "WarrantyEndDate" DATE,
    "Notes" TEXT,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_MaintenanceRecords_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MaintenanceRecords_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_MaintenanceRecords_VehicleId" ON "MaintenanceRecords" ("VehicleId");
CREATE INDEX "IX_MaintenanceRecords_MaintenanceDate" ON "MaintenanceRecords" ("MaintenanceDate");
CREATE INDEX "IX_MaintenanceRecords_Type" ON "MaintenanceRecords" ("Type");
```

### 8. Table MileageRecords (Historique Kilométrage)
```sql
CREATE TABLE "MileageRecords" (
    "Id" SERIAL PRIMARY KEY,
    "VehicleId" INTEGER NOT NULL,
    "Mileage" INTEGER NOT NULL,
    "RecordedDate" DATE NOT NULL,
    "PhotoUrl" VARCHAR(500), -- Photo du compteur
    "Notes" VARCHAR(500),
    "RecordedBy" VARCHAR(450),
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "FK_MileageRecords_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MileageRecords_RecordedBy" FOREIGN KEY ("RecordedBy") REFERENCES "AspNetUsers"("Id"),
    CONSTRAINT "CK_MileageRecords_Mileage" CHECK ("Mileage" >= 0)
);

CREATE INDEX "IX_MileageRecords_VehicleId" ON "MileageRecords" ("VehicleId");
CREATE INDEX "IX_MileageRecords_RecordedDate" ON "MileageRecords" ("RecordedDate");
```

### 9. Table Violations (Infractions)
```sql
CREATE TABLE "Violations" (
    "Id" SERIAL PRIMARY KEY,
    "DriverId" INTEGER NOT NULL,
    "VehicleId" INTEGER,
    "ViolationType" INTEGER NOT NULL, -- 1=Speeding, 2=Parking, 3=RedLight, etc.
    "ViolationDate" DATE NOT NULL,
    "Location" VARCHAR(500),
    "Description" TEXT NOT NULL,
    "FineAmount" DECIMAL(8,2),
    "PointsDeducted" INTEGER DEFAULT 0,
    "Status" INTEGER DEFAULT 1, -- 1=Received, 2=Paid, 3=Contested, 4=Cancelled
    "PaymentDate" DATE,
    "ContestDate" DATE,
    "TicketNumber" VARCHAR(50),
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "CreatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_Violations_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers"("Id"),
    CONSTRAINT "FK_Violations_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id"),
    CONSTRAINT "FK_Violations_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_Violations_DriverId" ON "Violations" ("DriverId");
CREATE INDEX "IX_Violations_ViolationDate" ON "Violations" ("ViolationDate");
CREATE INDEX "IX_Violations_Status" ON "Violations" ("Status");
```

### 10. Table Alerts (Alertes)
```sql
CREATE TABLE "Alerts" (
    "Id" SERIAL PRIMARY KEY,
    "Type" INTEGER NOT NULL, -- 1=TechnicalInspection, 2=Insurance, 3=Maintenance, etc.
    "Severity" INTEGER DEFAULT 2, -- 1=Critical, 2=Warning, 3=Info
    "Title" VARCHAR(200) NOT NULL,
    "Message" TEXT NOT NULL,
    "VehicleId" INTEGER,
    "DriverId" INTEGER,
    "DueDate" DATE,
    "DaysRemaining" INTEGER,
    "IsProcessed" BOOLEAN DEFAULT FALSE,
    "ProcessedDate" TIMESTAMPTZ,
    "ProcessedBy" VARCHAR(450),
    "EmailSent" BOOLEAN DEFAULT FALSE,
    "EmailSentDate" TIMESTAMPTZ,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT "FK_Alerts_Vehicle" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Alerts_Driver" FOREIGN KEY ("DriverId") REFERENCES "Drivers"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Alerts_ProcessedBy" FOREIGN KEY ("ProcessedBy") REFERENCES "AspNetUsers"("Id")
);

CREATE INDEX "IX_Alerts_Type" ON "Alerts" ("Type");
CREATE INDEX "IX_Alerts_Severity" ON "Alerts" ("Severity");
CREATE INDEX "IX_Alerts_IsProcessed" ON "Alerts" ("IsProcessed");
CREATE INDEX "IX_Alerts_DueDate" ON "Alerts" ("DueDate");
```

### 11. Table de Configuration Système
```sql
CREATE TABLE "SystemSettings" (
    "Id" SERIAL PRIMARY KEY,
    "SettingKey" VARCHAR(100) UNIQUE NOT NULL,
    "SettingValue" TEXT,
    "Description" TEXT,
    "DataType" VARCHAR(20) DEFAULT 'string', -- string, int, bool, json
    "Category" VARCHAR(50), -- Email, Alerts, System, etc.
    "IsEditable" BOOLEAN DEFAULT TRUE,
    
    -- Audit
    "CreatedAt" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ,
    "UpdatedBy" VARCHAR(450),
    
    CONSTRAINT "FK_SystemSettings_UpdatedBy" FOREIGN KEY ("UpdatedBy") REFERENCES "AspNetUsers"("Id")
);

-- Insertion des paramètres par défaut
INSERT INTO "SystemSettings" ("SettingKey", "SettingValue", "Description", "Category") VALUES
('EMAIL_SMTP_HOST', 'smtp.gmail.com', 'Serveur SMTP pour envoi d''emails', 'Email'),
('EMAIL_SMTP_PORT', '587', 'Port SMTP', 'Email'),
('EMAIL_FROM_ADDRESS', 'noreply@fleetsync.com', 'Adresse email expéditeur', 'Email'),
('ALERT_CT_DAYS_BEFORE', '60,30,15', 'Jours avant CT pour envoi alertes (CSV)', 'Alerts'),
('ALERT_INSURANCE_DAYS_BEFORE', '30,15', 'Jours avant expiration assurance', 'Alerts'),
('MAINTENANCE_DEFAULT_INTERVAL_KM', '15000', 'Intervalle maintenance par défaut (km)', 'Maintenance'),
('MAINTENANCE_DEFAULT_INTERVAL_MONTHS', '12', 'Intervalle maintenance par défaut (mois)', 'Maintenance');
```

## Fonctions et Triggers PostgreSQL

### 1. Fonction de mise à jour automatique des alertes
```sql
CREATE OR REPLACE FUNCTION update_vehicle_alerts()
RETURNS TRIGGER AS $$
BEGIN
    -- Supprimer les anciennes alertes pour ce véhicule
    DELETE FROM "Alerts" WHERE "VehicleId" = NEW."Id" AND "Type" IN (1, 2, 3);
    
    -- Créer alerte CT si nécessaire
    IF NEW."NextTechnicalInspection" <= CURRENT_DATE + INTERVAL '60 days' THEN
        INSERT INTO "Alerts" ("Type", "Severity", "Title", "Message", "VehicleId", "DueDate", "DaysRemaining")
        VALUES (
            1, -- TechnicalInspection
            CASE WHEN NEW."NextTechnicalInspection" <= CURRENT_DATE + INTERVAL '15 days' THEN 1 ELSE 2 END,
            'Contrôle technique à prévoir',
            'Le véhicule ' || NEW."LicensePlate" || ' doit passer le contrôle technique le ' || NEW."NextTechnicalInspection",
            NEW."Id",
            NEW."NextTechnicalInspection",
            EXTRACT(DAY FROM NEW."NextTechnicalInspection" - CURRENT_DATE)
        );
    END IF;
    
    -- Créer alerte assurance si nécessaire
    IF NEW."InsuranceExpiryDate" <= CURRENT_DATE + INTERVAL '30 days' THEN
        INSERT INTO "Alerts" ("Type", "Severity", "Title", "Message", "VehicleId", "DueDate", "DaysRemaining")
        VALUES (
            2, -- Insurance
            CASE WHEN NEW."InsuranceExpiryDate" <= CURRENT_DATE + INTERVAL '15 days' THEN 1 ELSE 2 END,
            'Assurance à renouveler',
            'L''assurance du véhicule ' || NEW."LicensePlate" || ' expire le ' || NEW."InsuranceExpiryDate",
            NEW."Id",
            NEW."InsuranceExpiryDate",
            EXTRACT(DAY FROM NEW."InsuranceExpiryDate" - CURRENT_DATE)
        );
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Créer le trigger
CREATE TRIGGER trigger_update_vehicle_alerts
    AFTER INSERT OR UPDATE ON "Vehicles"
    FOR EACH ROW
    EXECUTE FUNCTION update_vehicle_alerts();
```

### 2. Fonction de génération automatique du numéro de sinistre
```sql
CREATE OR REPLACE FUNCTION generate_incident_number()
RETURNS TRIGGER AS $$
DECLARE
    year_suffix VARCHAR(4);
    sequence_number INTEGER;
    incident_number VARCHAR(20);
BEGIN
    -- Récupérer l'année courante
    year_suffix := EXTRACT(YEAR FROM CURRENT_DATE)::VARCHAR;
    
    -- Récupérer le prochain numéro de séquence pour cette année
    SELECT COALESCE(MAX(CAST(SUBSTRING("IncidentNumber" FROM 10) AS INTEGER)), 0) + 1
    INTO sequence_number
    FROM "Incidents"
    WHERE "IncidentNumber" LIKE 'INC-' || year_suffix || '-%';
    
    -- Générer le numéro avec padding
    incident_number := 'INC-' || year_suffix || '-' || LPAD(sequence_number::VARCHAR, 6, '0');
    
    NEW."IncidentNumber" := incident_number;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Créer le trigger
CREATE TRIGGER trigger_generate_incident_number
    BEFORE INSERT ON "Incidents"
    FOR EACH ROW
    EXECUTE FUNCTION generate_incident_number();
```

### 3. Vue pour les statistiques du tableau de bord
```sql
CREATE VIEW "DashboardStats" AS
SELECT
    (SELECT COUNT(*) FROM "Vehicles" WHERE "Status" = 1) AS "ActiveVehicles",
    (SELECT COUNT(*) FROM "Vehicles" WHERE "Status" = 2) AS "VehiclesInMaintenance",
    (SELECT COUNT(*) FROM "Drivers" WHERE "Status" = 1) AS "ActiveDrivers",
    (SELECT COUNT(*) FROM "Incidents" WHERE "Status" IN (1, 2)) AS "ActiveIncidents",
    (SELECT COUNT(*) FROM "Alerts" WHERE "IsProcessed" = FALSE AND "Severity" = 1) AS "CriticalAlerts",
    (SELECT COUNT(*) FROM "Alerts" WHERE "IsProcessed" = FALSE) AS "TotalAlerts",
    (SELECT SUM("CurrentMileage") FROM "Vehicles") AS "TotalMileage",
    (SELECT ROUND(AVG("CurrentMileage"), 0) FROM "Vehicles" WHERE "Status" = 1) AS "AverageMileage";
```

## Scripts d'Initialisation

### 1. Script de création complète
```sql
-- Création de la base de données
CREATE DATABASE fleetsyncmanager
    WITH ENCODING 'UTF8'
    LC_COLLATE = 'fr_FR.UTF-8'
    LC_CTYPE = 'fr_FR.UTF-8';

-- Connexion à la base
\c fleetsyncmanager;

-- Créer les extensions nécessaires
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- [Insérer ici tous les scripts CREATE TABLE...]
```

### 2. Script de données de test
```sql
-- Données de test pour développement
INSERT INTO "Drivers" ("FirstName", "LastName", "DateOfBirth", "Email", "PhoneNumber", "HireDate", "LicenseObtainedDate", "LicenseExpiryDate", "LicenseNumber", "LicenseTypes", "CreatedBy") VALUES
('Jean', 'DUPONT', '1980-06-15', 'jean.dupont@example.com', '0612345678', '2020-01-15', '1998-03-20', '2028-03-20', 'B123456789', '["B"]', '1'),
('Marie', 'MARTIN', '1975-09-22', 'marie.martin@example.com', '0623456789', '2019-03-10', '1993-07-15', '2026-07-15', 'C987654321', '["B", "C1"]', '1'),
('Pierre', 'BERNARD', '1985-12-08', 'pierre.bernard@example.com', '0634567890', '2021-06-01', '2003-11-10', '2030-11-10', 'D456789123', '["B", "D"]', '1');

INSERT INTO "Vehicles" ("LicensePlate", "Vin", "Brand", "Model", "Year", "FuelType", "Status", "CurrentMileage", "PurchaseDate", "NextTechnicalInspection", "InsuranceExpiryDate", "AssignedDriverId", "CreatedBy") VALUES
('AB-123-CD', 'VF1KMXXXXXX123456', 'Peugeot', '308 SW', 2020, 2, 1, 85420, '2020-03-15', '2025-09-15', '2025-12-31', 1, '1'),
('EF-456-GH', 'VF1REXXXXXX654321', 'Renault', 'Kangoo', 2019, 2, 1, 127350, '2019-05-20', '2025-11-20', '2025-10-15', 2, '1'),
('IJ-789-KL', 'VF7IVXXXXXX789123', 'Iveco', 'Daily', 2021, 2, 1, 45780, '2021-02-10', '2026-02-10', '2026-01-20', 3, '1');
```

## Optimisations et Performance

### 1. Index de performance supplémentaires
```sql
-- Index composites pour requêtes fréquentes
CREATE INDEX "IX_Vehicles_Status_AssignedDriver" ON "Vehicles" ("Status", "AssignedDriverId");
CREATE INDEX "IX_Alerts_Unprocessed_Severity" ON "Alerts" ("IsProcessed", "Severity") WHERE "IsProcessed" = FALSE;
CREATE INDEX "IX_Incidents_Vehicle_Date" ON "Incidents" ("VehicleId", "IncidentDate");
CREATE INDEX "IX_MaintenanceRecords_Vehicle_Date" ON "MaintenanceRecords" ("VehicleId", "MaintenanceDate");
```

### 2. Configuration PostgreSQL recommandée
```sql
-- Configuration pour optimiser les performances
ALTER SYSTEM SET shared_buffers = '256MB';
ALTER SYSTEM SET effective_cache_size = '1GB';
ALTER SYSTEM SET work_mem = '4MB';
ALTER SYSTEM SET maintenance_work_mem = '64MB';
ALTER SYSTEM SET max_connections = '100';
```

Ce schéma de base de données PostgreSQL fournit une fondation robuste et évolutive pour le MVP FleetSyncManager, avec des relations bien définies, des contraintes d'intégrité et des optimisations de performance appropriées.
