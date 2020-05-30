
using DatabaseConnection.Configurations;
using IntegrationApi.Entries;
using Microsoft.EntityFrameworkCore;
using Models;
namespace DatabaseConnection
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new EntryConfiguration());
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}