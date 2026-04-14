using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

namespace SGC.Clinica.Application.Schedules.Queries
{
    public record GetScheduleByIdQuery(int Id) : IRequest<ScheduleDto>;
}
