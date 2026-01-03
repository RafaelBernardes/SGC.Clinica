using FluentValidation;
using SGC.Clinica.Api.Application.Schedules.Commands;
using SGC.Clinica.Api.Domain.Enums;
using SGC.Clinica.Api.Repositories.Interfaces;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Schedules.Validators
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
