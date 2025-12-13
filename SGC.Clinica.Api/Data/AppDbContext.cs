using Microsoft.EntityFrameworkCore;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Physioterapist> Physiotherapists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Professional>().ToTable("Professionals");
        }
    }
}