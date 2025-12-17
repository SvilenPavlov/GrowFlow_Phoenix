using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GrowFlow_Phoenix.Data
{
    public class PhoenixDbContext : DbContext
    {
        public PhoenixDbContext(DbContextOptions<PhoenixDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<LeviathanSnapshotEntry> LeviathanSnapshotEntries => Set<LeviathanSnapshotEntry>();
    }
}
