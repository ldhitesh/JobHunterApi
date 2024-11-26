using Microsoft.EntityFrameworkCore;
using JobHunterApi.Models;

namespace JobHunterApi.Database
{
    public class CompaniesDbContext : DbContext
    {
        public CompaniesDbContext(DbContextOptions<CompaniesDbContext> options)
            : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReferencesModel>()
                .HasKey(e => e.Id);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RegisterModel>()
                .HasKey(e => e.Id);
        }

        public DbSet<ReferencesModel> CompanyReferences { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<RegisterModel> PendingRegistrations { get; set; }

    }
}
