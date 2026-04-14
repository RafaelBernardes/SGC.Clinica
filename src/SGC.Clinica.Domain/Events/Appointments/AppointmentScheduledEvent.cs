namespace SGC.Clinica.Domain.Events.Appointments
{
    public record AppointmentScheduledEvent(int AppointmentId, int PatientId, int ProfessionalId, DateTime ScheduledDate) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
