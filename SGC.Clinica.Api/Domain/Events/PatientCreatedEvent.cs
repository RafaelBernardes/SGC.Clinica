using MediatR;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Domain.Events
{
    public record PatientCreatedEvent(Patient Patient) : INotification;
}