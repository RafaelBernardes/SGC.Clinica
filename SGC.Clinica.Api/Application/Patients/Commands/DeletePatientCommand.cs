using MediatR;

namespace SGC.Clinica.Api.Application.Patients.Commands
{
    public record DeletePatientCommand(int PatientId) : IRequest<Unit>;
}