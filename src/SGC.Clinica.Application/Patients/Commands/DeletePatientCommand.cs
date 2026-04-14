using MediatR;

namespace SGC.Clinica.Application.Patients.Commands
{
    public record DeletePatientCommand(int PatientId) : IRequest<Unit>;
}
