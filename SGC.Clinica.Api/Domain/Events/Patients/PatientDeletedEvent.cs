using MediatR;

namespace SGC.Clinica.Api.Domain.Events.Patients
{
    public record PatientDeletedEvent(int PatientId) : INotification; 
}