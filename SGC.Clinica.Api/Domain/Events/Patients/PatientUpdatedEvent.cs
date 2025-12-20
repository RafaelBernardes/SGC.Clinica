using MediatR;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Domain.Events.Patients
{
    public record PatientUpdatedEvent(Patient Patient) : INotification;
}