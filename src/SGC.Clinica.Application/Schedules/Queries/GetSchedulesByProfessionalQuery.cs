using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;

namespace SGC.Clinica.Api.Application.Schedules.Queries
{
    public record GetSchedulesByProfessionalQuery(int professionalId) : IRequest<List<ScheduleDto>>;
}