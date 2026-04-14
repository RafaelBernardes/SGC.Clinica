using MediatR;
using SGC.Clinica.Application.Patients.Dtos;

namespace SGC.Clinica.Application.Patients.Queries
{
    public record GetPatientsListQuery() : IRequest<IEnumerable<PatientDto>>;
}
