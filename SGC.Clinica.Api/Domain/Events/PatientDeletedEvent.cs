using MediatR;

namespace SGC.Clinica.Api.Domain.Events
{
    public record PatientDeletedEvent(int PatientId) : INotification; 
}