using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;


namespace SGC.Clinica.Api.Domain.Events.Appointments
{
    public record AppointmentScheduledEvent(AppointmentDto Appointment) : INotification, IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}