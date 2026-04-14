using FluentValidation;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Domain.Enums;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;

namespace SGC.Clinica.Application.Schedules.Validators
{
    public class CancelAppointmentCommandValidation : AbstractValidator<CancelAppointmentCommand>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        public CancelAppointmentCommandValidation(IBaseRepository<AppointmentModel> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;

            RuleFor(x => x.AppointmentId).NotEmpty()
            .MustAsync(AppointmentExistsAndIsActive).WithMessage("O agendamento especificado não existe ou está inativo.");
        }

        private async Task<bool> AppointmentExistsAndIsActive(int appointmentId, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            return appointment != null && appointment.Status == AppointmentStatus.Pending;
        }
    }
}

