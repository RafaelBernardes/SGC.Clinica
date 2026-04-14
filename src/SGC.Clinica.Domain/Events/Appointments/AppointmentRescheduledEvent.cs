namespace SGC.Clinica.Domain.Events.Appointments
{ 
    public record AppointmentRescheduledEvent(int AppointmentId, DateTime NewDate) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}   
