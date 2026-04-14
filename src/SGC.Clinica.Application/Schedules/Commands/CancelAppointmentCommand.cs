using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

namespace SGC.Clinica.Application.Schedules.Commands
{
    public record CancelAppointmentCommand(int AppointmentId, string Reason) : IRequest<AppointmentDto>;
}

