using Microsoft.EntityFrameworkCore;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Models;

namespace SGC.Clinica.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Physioterapist> Physioterapists { get; set; }
    }
}