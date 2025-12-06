using FluentValidation;

namespace SGC.Clinica.Api.Application.Patients.Validators
{
    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.PatientDto.Name)
                .NotEmpty().WithMessage("The full name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");
            
            RuleFor(x => x.PatientDto.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of Birth must be in the past.");
            
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("PatientId must be a positive integer.");
        }
    }
}