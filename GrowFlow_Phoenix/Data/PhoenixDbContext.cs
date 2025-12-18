using GrowFlow_Phoenix.Models;
using GrowFlow_Phoenix.Models.IModels;
using GrowFlow_Phoenix.Models.Utility;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GrowFlow_Phoenix.Data
{
    public class PhoenixDbContext : DbContext
    {
        public PhoenixDbContext(DbContextOptions<PhoenixDbContext> options)
            : base(options) { }
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().OwnsOne(e => e.AuditRecord);
            // repeat or use a convention
        }
        private void ApplyAuditInfo()
        {
            var utcNow = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added && entry.Entity is IAuditable<AuditRecord> added)
                {
                    added.AuditRecord.CreatedAt = utcNow;
                }
                else if (entry.State == EntityState.Modified && entry.Entity is IAuditable<AuditRecord> modded)
                {
                    modded.AuditRecord.LastModifiedAt = utcNow;
                }
            }
        }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeviathanSnapshotEntry> LeviathanSnapshotEntries => Set<LeviathanSnapshotEntry>();
        public DbSet<EmployeeExternalId> EmployeeExternalIds => Set<EmployeeExternalId>();

    }
}
