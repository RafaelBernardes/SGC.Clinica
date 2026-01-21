using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Application.Schedules.Commands
{
    public record CreateScheduleCommand(CreateScheduleDto ScheduleDto) : IRequest<Schedule>;
}