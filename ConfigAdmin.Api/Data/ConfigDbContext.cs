using Microsoft.EntityFrameworkCore;
using ConfigAdmin.Api.Entities;

namespace ConfigAdmin.Api.Data
{
    public class ConfigDbContext : DbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options)
        : base(options)
        {
        }

        public DbSet<ConfigRecord> Configurations => Set<ConfigRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigRecord>().ToTable("Configurations");
        }
    }
}
