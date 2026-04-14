using FluentValidation;
using SGC.Clinica.Application.Patients.Commands;

namespace SGC.Clinica.Application.Patients.Validators
{
    public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientCommandValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("PatientId must be a positive integer.");
        }
    }
}
