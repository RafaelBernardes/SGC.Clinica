using FluentValidation;
using SGC.Clinica.Api.Application.Schedules.Commands;
using SGC.Clinica.Api.Domain.Enums;
using SGC.Clinica.Api.Repositories.Interfaces;
using SGC.Clinica.Api.Repositories.Specifications;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Schedules.Validators
{
    public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;

        public RescheduleAppointmentCommandValidator(IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.AppointmentId)
                .GreaterThan(0).WithMessage("O ID do Agendamento deve ser válido.")
                .MustAsync(AppointmentExistsAndCanBeRescheduled).WithMessage("O agendamento não existe ou seu status não permite reagendamento.");

            RuleFor(x => x.NewScheduledDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("A nova data do agendamento deve ser no futuro.");

            RuleFor(x => x)
                .CustomAsync(ValidateNoConflict);
        }

        private async Task<bool> AppointmentExistsAndCanBeRescheduled(int appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            return appointment != null && (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Confirmed);
        }

        private async Task ValidateNoConflict(RescheduleAppointmentCommand command, ValidationContext<RescheduleAppointmentCommand> context, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.AppointmentId, cancellationToken);
            if (appointment == null) return;

            var proposedStartTime = command.NewScheduledDate;
            var proposedEndTime = proposedStartTime.Add(appointment.Duration);

            var conflictingSpec = new ConflictingAppointmentSpec(appointment.ProfessionalId, proposedStartTime, proposedEndTime, appointment.Id);
            var conflictingAppointment = await _appointmentRepository.FirstOrDefaultAsync(conflictingSpec, cancellationToken);

            if (conflictingAppointment != null)
            {
                context.AddFailure("Conflito de horário. O profissional já possui um agendamento neste novo horário.");
            }
        }
    }
}
