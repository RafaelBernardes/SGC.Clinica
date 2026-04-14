using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;
using SGC.Clinica.Domain.Models;

namespace SGC.Clinica.Application.Schedules.Commands
{
    public record CreateScheduleCommand(CreateScheduleDto ScheduleDto) : IRequest<Schedule>;
}
