using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;

namespace SGC.Clinica.Api.Application.Appointment.Queries
{
    public record GetAppointmentsByProfessionalQuery(int professionalId, DateTime? startDate, DateTime? endDate) : IRequest<List<AppointmentDto>>;
}