namespace SGC.Clinica.Domain.Events.Patients
{
    public record PatientDeletedEvent(int PatientId, string PatientName, string Email) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}
