using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;

namespace SGC.Clinica.Api.Application.Appointment.Queries
{
    public record GetAppointmentByIdQuery(int Id) : IRequest<AppointmentDto?>;
}