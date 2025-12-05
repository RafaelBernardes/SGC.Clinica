using Microsoft.EntityFrameworkCore;
using SGC.Clinica.Api.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Physioterapist> Physioterapists { get; set; }
}