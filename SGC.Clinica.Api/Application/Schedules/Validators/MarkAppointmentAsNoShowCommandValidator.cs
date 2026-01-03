using FluentValidation;
using SGC.Clinica.Api.Application.Schedules.Commands;
using SGC.Clinica.Api.Domain.Enums;
using SGC.Clinica.Api.Repositories.Interfaces;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Schedules.Validators
{
    public class MarkAppointmentAsNoShowCommandValidator : AbstractValidator<MarkAppointmentAsNoShowCommand>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        public MarkAppointmentAsNoShowCommandValidator(IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.AppointmentId).NotEmpty()
            .MustAsync(AppointmentExistsAndIsPendingOrConfirmed).WithMessage("O agendamento especificado não existe ou não está pendente/confirmado.");
        }

        private async Task<bool> AppointmentExistsAndIsPendingOrConfirmed(int appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            return appointment != null && (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Confirmed);
        }
    }
}
