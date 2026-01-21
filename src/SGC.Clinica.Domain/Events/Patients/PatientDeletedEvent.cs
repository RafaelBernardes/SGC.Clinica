using MediatR;

namespace SGC.Clinica.Api.Domain.Events.Patients
{
    public record PatientDeletedEvent(int PatientId, string PatientName, string Email) : INotification, IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}