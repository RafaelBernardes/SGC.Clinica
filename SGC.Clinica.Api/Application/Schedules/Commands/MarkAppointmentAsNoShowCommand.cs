using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;

namespace SGC.Clinica.Api.Application.Schedules.Commands
{
    public record MarkAppointmentAsNoShowCommand(int AppointmentId) : IRequest<AppointmentDto>;
}