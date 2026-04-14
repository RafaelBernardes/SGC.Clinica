namespace SGC.Clinica.Domain.Events.Patients
{
    public record PatientCreatedEvent(int PatientId, string PatientName, string Email) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
