using MediatR;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Models;

namespace SGC.Clinica.Api.Application.Patients.Commands
{
    public record AddPatientCommand(AddPatientDto PatientDto) : IRequest<Patient>;
}
