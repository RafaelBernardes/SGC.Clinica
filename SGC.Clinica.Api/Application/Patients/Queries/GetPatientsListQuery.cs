using MediatR;
using SGC.Clinica.Api.Application.Patients.Dtos;

namespace SGC.Clinica.Api.Application.Patients.Queries
{
    public record GetPatientsListQuery() : IRequest<IEnumerable<PatientDto>>;
}