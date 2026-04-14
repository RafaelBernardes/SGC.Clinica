namespace SGC.Clinica.Domain.Events.Appointments
{ 
    public record AppointmentCompletedEvent(int AppointmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
