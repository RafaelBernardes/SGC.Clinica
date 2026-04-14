using MediatR;

namespace SGC.Clinica.Application.Schedules.Queries
{
    public record CheckAvailabilityQuery(long TimeSlotId) : IRequest<bool>;
}
