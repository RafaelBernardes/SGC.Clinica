using MediatR;
using SGC.Clinica.Application.Patients.Dtos;
using SGC.Clinica.Domain.Models;


namespace SGC.Clinica.Application.Patients.Commands
{
    public record AddPatientCommand(AddPatientDto PatientDto) : IRequest<Patient>;
}

