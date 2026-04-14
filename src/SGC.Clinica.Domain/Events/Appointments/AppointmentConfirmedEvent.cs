namespace SGC.Clinica.Domain.Events.Appointments
{
    public record AppointmentConfirmedEvent(int AppointmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
