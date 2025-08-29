using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Laroche.FleetManager.Domain.Entities;
using System.Linq.Expressions;

namespace Laroche.FleetManager.Infrastructure.Data
{
    /// <summary>
    /// Contexte de base de données principal pour FleetSyncManager
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region DbSets - Entités métier

        /// <summary>
        /// Véhicules de la flotte
        /// </summary>
        public DbSet<Vehicle> Vehicles { get; set; } = null!;

        /// <summary>
        /// Conducteurs
        /// </summary>
        public DbSet<Driver> Drivers { get; set; } = null!;

        /// <summary>
        /// Incidents et sinistres
        /// </summary>
        public DbSet<Incident> Incidents { get; set; } = null!;

        /// <summary>
        /// Opérations de maintenance
        /// </summary>
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;

        /// <summary>
        /// Audit des connexions utilisateur
        /// </summary>
        public DbSet<LoginAudit> LoginAudits { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuration des tables Identity avec préfixes
            ConfigureIdentityTables(builder);

            // Configuration des contraintes globales
            ConfigureGlobalConstraints(builder);
        }

        /// <summary>
        /// Configuration des tables ASP.NET Identity avec préfixes personnalisés
        /// </summary>
        private static void ConfigureIdentityTables(ModelBuilder builder)
        {
            // Préfixer les tables Identity
            builder.Entity<ApplicationUser>()
                .ToTable("Users");
            
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>()
                .ToTable("Roles");
                
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>()
                .ToTable("UserRoles");
                
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>()
                .ToTable("UserClaims");
                
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>()
                .ToTable("UserLogins");
                
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>()
                .ToTable("UserTokens");
                
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>()
                .ToTable("RoleClaims");

            // Configuration des relations pour ApplicationUser
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.LoginAudits)
                .WithOne(la => la.User)
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Driver)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.DriverId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        /// <summary>
        /// Configuration des contraintes et conventions globales
        /// </summary>
        private static void ConfigureGlobalConstraints(ModelBuilder builder)
        {
            // Configuration des propriétés DateTime pour PostgreSQL
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    // Forcer les DateTime en UTC pour PostgreSQL
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamptz");
                    }
                    
                    // Configuration des propriétés string par défaut
                    if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                    {
                        property.SetMaxLength(255); // Longueur par défaut
                    }
                }
            }
        }

        /// <summary>
        /// Override SaveChangesAsync pour implémenter l'audit automatique
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Override SaveChanges pour implémenter l'audit automatique
        /// </summary>
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        /// <summary>
        /// Mise à jour automatique des champs d'audit
        /// </summary>
        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // Propriété CreatedAt pour les nouvelles entités
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entry.Properties
                        .FirstOrDefault(p => p.Metadata.Name == "CreatedAt");
                    if (createdAtProperty != null)
                    {
                        createdAtProperty.CurrentValue = currentTime;
                    }
                }

                // Propriété UpdatedAt pour les entités modifiées
                if (entry.State == EntityState.Modified)
                {
                    var updatedAtProperty = entry.Properties
                        .FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
                    if (updatedAtProperty != null)
                    {
                        updatedAtProperty.CurrentValue = currentTime;
                    }
                }
            }
        }
    }
}
