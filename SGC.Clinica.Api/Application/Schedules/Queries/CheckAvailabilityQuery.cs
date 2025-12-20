using MediatR;

namespace SGC.Clinica.Api.Application.Schedules.Queries
{
    public record CheckAvailabilityQuery(int professionalId, DateTime scheduleDate, int durationMinutes) : IRequest<bool>;
}