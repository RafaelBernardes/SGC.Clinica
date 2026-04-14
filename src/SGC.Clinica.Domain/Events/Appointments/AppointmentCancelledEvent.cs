namespace SGC.Clinica.Domain.Events.Appointments
{ 
    public record AppointmentCancelledEvent(int AppointmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}
