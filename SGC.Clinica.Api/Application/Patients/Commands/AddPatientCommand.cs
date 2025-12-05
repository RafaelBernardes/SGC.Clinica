using MediatR;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Models;

namespace SGC.Clinica.Api.Application.Patients.Commands
{
    public record AddPatientCommand : IRequest<Patient>
    {
        public AddPatientDto PatientDto { get; set; }

        public AddPatientCommand(AddPatientDto patientDto)
        {
            PatientDto = patientDto;
        }
    }
}
