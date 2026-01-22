using MediatR;

namespace SGC.Clinica.Api.Domain.Events.Appointments
{
    public record AppointmentScheduledEvent(int AppointmentId, string PatientName, string PatientEmail, string DoctorName, DateTime ScheduledDate) : INotification, IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}