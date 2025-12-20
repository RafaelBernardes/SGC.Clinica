using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;
using System.Collections.Generic;

namespace SGC.Clinica.Api.Application.Appointment.Queries
{
    public record GetAppointmentsByPatientQuery(int PatientId) : IRequest<IEnumerable<AppointmentDto>>;
}