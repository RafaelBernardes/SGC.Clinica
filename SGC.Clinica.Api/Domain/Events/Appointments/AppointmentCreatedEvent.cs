using MediatR;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Domain.Events.Appointments
{
    public record AppointmentCreatedEvent(Appointment Appointment) : INotification;
}