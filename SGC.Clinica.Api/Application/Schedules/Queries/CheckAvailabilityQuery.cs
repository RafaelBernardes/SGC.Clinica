using MediatR;

namespace SGC.Clinica.Api.Application.Schedules.Queries
{
    public record CheckAvailabilityQuery(long TimeSlotId) : IRequest<bool>;
}