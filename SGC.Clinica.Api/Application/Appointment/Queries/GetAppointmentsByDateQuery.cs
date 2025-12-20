using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;

namespace SGC.Clinica.Api.Application.Appointment.Queries
{
    public record GetAppointmentsByDateQuery(DateTime date, int? professionalId) : IRequest<IEnumerable<AppointmentDto>>;
}