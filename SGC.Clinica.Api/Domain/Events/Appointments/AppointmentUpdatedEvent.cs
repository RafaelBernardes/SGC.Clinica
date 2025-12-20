using MediatR;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Domain.Events.Appointments
{
    public record AppointmentUpdatedEvent(Appointment Appointment) : INotification;
}