using MediatR;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Domain.Events.Patients
{
    public record PatientUpdatedEvent(int PatientId, string PatientName, string Email) : INotification, IDomainEvent
    {
        public DateTime OccurredOn => DateTime.UtcNow;
    }
}