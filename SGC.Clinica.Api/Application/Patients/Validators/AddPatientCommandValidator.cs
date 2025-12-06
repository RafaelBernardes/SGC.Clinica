using FluentValidation;
using SGC.Clinica.Api.Application.Patients.Commands;

namespace SGC.Clinica.Api.Application.Patients.Validators
{
    public class AddPatientCommandValidator : AbstractValidator<AddPatientCommand>
    {
        public AddPatientCommandValidator()
        {
            RuleFor(x => x.PatientDto.Name)
                .NotEmpty().WithMessage("The full name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.PatientDto.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of Birth must be in the past.");
        }
    }
}