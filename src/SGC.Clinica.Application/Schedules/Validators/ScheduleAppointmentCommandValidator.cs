using FluentValidation;
using SGC.Clinica.Api.Application.Schedules.Commands;
using SGC.Clinica.Api.Domain.Models;
using SGC.Clinica.Api.Infrastructure.Persistence.Repositories.Interfaces;
using SGC.Clinica.Api.Infrastructure.Persistence.Repositories.Specifications;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Schedules.Validators
{
    public class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IBaseRepository<Professional> _professionalRepository;
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;

        public ScheduleAppointmentCommandValidator(
            IBaseRepository<Patient> patientRepository, 
            IBaseRepository<Professional> professionalRepository,
            IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _patientRepository = patientRepository;
            _professionalRepository = professionalRepository;
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.PatientId)
                .GreaterThan(0).WithMessage("O ID do Paciente deve ser válido.")
                .MustAsync(PatientExistsAndIsActive).WithMessage("O Paciente especificado não existe ou está inativo.");

            RuleFor(x => x.ProfessionalId)
                .GreaterThan(0).WithMessage("O ID do Profissional deve ser válido.")
                .MustAsync(ProfessionalExistsAndIsActive).WithMessage("O Profissional especificado não existe ou está inativo.");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("A data do agendamento deve ser no futuro.");

            RuleFor(x => x.DurationInMinutes)
                .InclusiveBetween(15, 480).WithMessage("A duração da consulta deve ser entre 15 e 480 minutos.");
            
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("O motivo da consulta é obrigatório.")
                .MaximumLength(200).WithMessage("O motivo não pode exceder 200 caracteres.");

            RuleFor(x => x)
                .CustomAsync(ValidateNoConflict);
        }

        private async Task<bool> PatientExistsAndIsActive(int patientId, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            return patient != null && patient.Active;
        }

        private async Task<bool> ProfessionalExistsAndIsActive(int professionalId, CancellationToken cancellationToken)
        {
            var professional = await _professionalRepository.GetByIdAsync(professionalId, cancellationToken);
            return professional != null && professional.Active;
        }

        private async Task ValidateNoConflict(ScheduleAppointmentCommand command, ValidationContext<ScheduleAppointmentCommand> context, CancellationToken cancellationToken)
        {
            var proposedStartTime = command.ScheduledAt;
            var proposedEndTime = proposedStartTime.Add(TimeSpan.FromMinutes(command.DurationInMinutes));

            var conflictingSpec = new ConflictingAppointmentSpec(command.ProfessionalId, proposedStartTime, proposedEndTime);
            var conflictingAppointment = await _appointmentRepository.FirstOrDefaultAsync(conflictingSpec, cancellationToken);

            if (conflictingAppointment != null)
            {
                context.AddFailure("Conflito de horário. O profissional já possui um agendamento neste horário.");
            }
        }
    }
}
