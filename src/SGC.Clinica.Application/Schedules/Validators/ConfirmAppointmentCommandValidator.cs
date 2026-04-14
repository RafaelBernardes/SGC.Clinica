using FluentValidation;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Domain.Enums;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;

namespace SGC.Clinica.Application.Schedules.Validators
{
    public class ConfirmAppointmentCommandValidator : AbstractValidator<ConfirmAppointmentCommand>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        public ConfirmAppointmentCommandValidator(IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.AppointmentId).NotEmpty()
            .MustAsync(AppointmentExistsAndIsPending).WithMessage("O agendamento especificado não existe ou não está pendente.");
        }

        private async Task<bool> AppointmentExistsAndIsPending(int appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            return appointment != null && appointment.Status == AppointmentStatus.Pending;
        }
    }
}

