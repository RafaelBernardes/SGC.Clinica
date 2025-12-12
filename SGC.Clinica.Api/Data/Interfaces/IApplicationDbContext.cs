using Microsoft.EntityFrameworkCore;
using SGC.Clinica.Api.Models;

namespace SGC.Clinica.Api.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Patient> Patients { get; set; }
        DbSet<Physioterapist> Physiotherapists { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}