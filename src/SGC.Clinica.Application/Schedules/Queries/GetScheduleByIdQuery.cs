using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;

namespace SGC.Clinica.Api.Application.Schedules.Queries
{
    public record GetScheduleByIdQuery(int Id) : IRequest<ScheduleDto>;
}