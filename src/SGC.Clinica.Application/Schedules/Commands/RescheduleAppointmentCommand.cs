using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

namespace SGC.Clinica.Application.Schedules.Commands
{
    public record RescheduleAppointmentCommand(int AppointmentId, DateTime NewScheduledDate) : IRequest<AppointmentDto>;
}
