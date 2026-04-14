namespace SGC.Clinica.Domain.Events.Appointments
{ 
    public record AppointmentNoShowEvent(int AppointmentId) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}   
