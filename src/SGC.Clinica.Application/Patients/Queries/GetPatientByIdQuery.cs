namespace SGC.Clinica.Application.Patients.Queries
{
    using MediatR;
    using SGC.Clinica.Application.Patients.Dtos;

    public record GetPatientByIdQuery(int Id) : IRequest<PatientDto?>;
}
