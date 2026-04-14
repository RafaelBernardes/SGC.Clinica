namespace SGC.Clinica.Domain.Events.Patients
{
    public record PatientUpdatedEvent(int PatientId, string PatientName, string Email) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
