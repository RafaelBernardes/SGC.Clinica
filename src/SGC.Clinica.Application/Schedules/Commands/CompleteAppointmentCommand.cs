using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

namespace SGC.Clinica.Application.Schedules.Commands
{
    public record CompleteAppointmentCommand(int AppointmentId) : IRequest<AppointmentDto>;
}
