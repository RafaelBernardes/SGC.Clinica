using FluentValidation;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Domain.Enums;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;

namespace SGC.Clinica.Application.Schedules.Validators
{
    public class CompleteAppointmentCommandValidator : AbstractValidator<CompleteAppointmentCommand>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        public CompleteAppointmentCommandValidator(IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.AppointmentId).NotEmpty()
            .MustAsync(AppointmentExistsAndIsConfirmed).WithMessage("O agendamento especificado não existe ou não está confirmado.");
        }

        private async Task<bool> AppointmentExistsAndIsConfirmed(int appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            return appointment != null && appointment.Status == AppointmentStatus.Confirmed;
        }
    }
}

