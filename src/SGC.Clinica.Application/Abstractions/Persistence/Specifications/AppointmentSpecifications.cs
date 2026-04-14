using SGC.Clinica.Domain.Enums;
using SGC.Clinica.Domain.Models;

namespace SGC.Clinica.Application.Abstractions.Persistence.Specifications
{
    public class ConflictingAppointmentSpec : Specification<Appointment>
    {
        public ConflictingAppointmentSpec(int professionalId, DateTime proposedStartTime, DateTime proposedEndTime, int? appointmentIdToExclude = null)
            : base(a =>
                a.ProfessionalId == professionalId &&
                (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed) &&
                a.ScheduledDate < proposedEndTime &&
                a.ScheduledDate.Add(a.Duration) > proposedStartTime &&
                (!appointmentIdToExclude.HasValue || a.Id != appointmentIdToExclude.Value))
        {
        }
    }
}

