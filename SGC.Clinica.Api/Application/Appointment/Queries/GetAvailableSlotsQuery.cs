using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;

namespace SGC.Clinica.Api.Application.Appointment.Queries
{
    public record GetAvailableSlotsQuery(int professionalId, DateTime startDate, DateTime endDate, int durationMinutes) : IRequest<List<AppointmentDto>>;
}