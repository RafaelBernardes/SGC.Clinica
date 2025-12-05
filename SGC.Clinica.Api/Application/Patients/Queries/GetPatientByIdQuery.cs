namespace SGC.Clinica.Api.Application.Patients.Queries
{
    using MediatR;
    using SGC.Clinica.Api.Dtos;

    public record GetPatientByIdQuery(int Id) : IRequest<PatientDto?>;
}