using Microsoft.EntityFrameworkCore;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Patient> Patients { get; set; }
        DbSet<Physioterapist> Physiotherapists { get; set; }
        DbSet<Appointment> Appointments { get; set; }
        DbSet<Schedule> Schedules { get; set; }
        DbSet<TimeSlot> TimeSlots { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}