using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

namespace SGC.Clinica.Application.Schedules.Queries
{
    public record GetSchedulesByProfessionalQuery(int professionalId) : IRequest<List<ScheduleDto>>;
}
