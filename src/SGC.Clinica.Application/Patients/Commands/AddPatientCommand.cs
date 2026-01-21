using MediatR;
using SGC.Clinica.Api.Application.Patients.Dtos;
using SGC.Clinica.Api.Domain.Models;


namespace SGC.Clinica.Api.Application.Patients.Commands
{
    public record AddPatientCommand(AddPatientDto PatientDto) : IRequest<Patient>;
}
