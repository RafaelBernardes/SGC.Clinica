using SGC.Clinica.Api.Domain.Enums;
using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Domain.Events.Appointments;
using SGC.Clinica.Api.Domain.Helpers;
using SGC.Clinica.Domain.Results;

namespace SGC.Clinica.Api.Domain.Models
{
    public sealed class Appointment
    {
        private Appointment(
            int patientId,
            int professionalId,
            DateTime scheduledDate,
            TimeSpan duration,
            string notes)
        {
            PatientId = patientId;
            ProfessionalId = professionalId;
            ScheduledDate = scheduledDate;
            Duration = duration;
            Notes = notes;
            Status = AppointmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public int PatientId { get; private set; }
        public int ProfessionalId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string Notes { get; private set; } = string.Empty;
        public AppointmentStatus Status { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public Patient Patient { get; private set; } = null!;
        public Professional Professional { get; private set; } = null!;
        private readonly List<IDomainEvent> _domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public static Result<Appointment> Create(
            int patientId,
            int professionalId,
            DateTime scheduledDate,
            TimeSpan duration,
            string notes)
        {
            var validation = ValidationHelper.Validate(
                ValidatePatientId(patientId),
                ValidateProfessionalId(professionalId),
                ValidateScheduleDate(scheduledDate),
                ValidateDuration(duration)
            );

            if (validation.IsFailure)
                return Result<Appointment>.Failure([.. validation.Errors]);

            var appointment = new Appointment(patientId, professionalId, scheduledDate, duration, notes);

            appointment.AddDomainEvent(new AppointmentScheduledEvent(appointment.PatientId, appointment.Patient.Name, appointment.Patient.Email, appointment.Professional.Name, appointment.ScheduledDate));
            
            return Result<Appointment>.Success(appointment);
        }

        public Result Confirm()
        {
            var validation = ValidateConfirmation();

            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            Status = AppointmentStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;

            //TODO: Adicionar evento de domínio para confirmação de agendamento

            return Result.Success();
        }

        public Result Cancel(string reason)
        {
            var validation = ValidateCancellation(reason);

            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            Status = AppointmentStatus.Cancelled;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;

            //TODO: Adicionar evento de domínio para cancelamento de agendamento

            return Result.Success();
        }

        public Result Complete()
        {
            var validation = ValidateCompletion();

            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            Status = AppointmentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;

            //TODO: Adicionar evento de domínio para conclusão de agendamento
            return Result.Success();
        }

        public Result Reschedule(DateTime newDate)
        {
            var validation = ValidationHelper.Validate(
                ValidateReschedule(),
                ValidateScheduleDate(newDate)
            );

            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            ScheduledDate = newDate;
            Status = AppointmentStatus.Rescheduled;
            UpdatedAt = DateTime.UtcNow;

            return Result.Success();
        }

        public void MarkAsNoShow()
        {
            Status = AppointmentStatus.NoShow;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasConflictWith(Appointment other)
        {
            if (other.Status == AppointmentStatus.Cancelled) return false;
            if (other.Id == Id) return false;

            var thisEnd = ScheduledDate.Add(Duration);
            var otherEnd = other.ScheduledDate.Add(other.Duration);

            return ScheduledDate < otherEnd && thisEnd > other.ScheduledDate;
        }

        #region Events

        public void ClearDomainEvents() => _domainEvents.Clear();

        private void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        #endregion

        #region guard clauses

        private static Result ValidatePatientId(int patientId)
        {
            var errors = new List<IError>();

            if (patientId <= 0)
                errors.Add(new ValidationError("InvalidPatientId", "PatientId inválido"));

            return errors.Any() 
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateProfessionalId(int professionalId)
        {
            var errors = new List<IError>();

        if (professionalId <= 0)
            errors.Add(new ValidationError("InvalidProfessionalId", "ProfessionalId inválido"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateScheduleDate(DateTime scheduledDate)
        {
            var errors = new List<IError>();

            if (scheduledDate <= DateTime.UtcNow)
                errors.Add(new ValidationError("PastDate", "Data do agendamento deve ser futura"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateDuration(TimeSpan duration)
        {
            var errors = new List<IError>();

            if (duration <= TimeSpan.Zero)
                errors.Add(new ValidationError("InvalidDuration", "Duração deve ser positiva"));

            if (duration < TimeSpan.FromMinutes(10) || duration > TimeSpan.FromHours(8))
                errors.Add(new ValidationError("DurationRange", "Duração deve estar entre 10 minutos e 8 horas"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private Result ValidateConfirmation()
        {
            if (Status != AppointmentStatus.Pending)
                return Result.Failure(new ValidationError("PatientInactive", "Apenas agendamentos pendentes podem ser confirmados"));

            return Result.Success();
        }

        private Result ValidateCancellation(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return Result.Failure(new ValidationError("CancellationReasonRequired", "Motivo do cancelamento é obrigatório."));


            if (Status == AppointmentStatus.Cancelled)
                return Result.Failure(new ValidationError("AlreadyCancelled", "Agendamento já está cancelado"));

            if (Status == AppointmentStatus.Completed)
                return Result.Failure(
                    new DomainError("AlreadyCompleted", "Agendamento já foi realizado"));

            return Result.Success();
        }

        private Result ValidateCompletion()
        {
            if (Status != AppointmentStatus.Confirmed)
                return Result.Failure(
                    new DomainError("NotConfirmed", "Apenas agendamentos confirmados podem ser completados"));

            return Result.Success();
        }

        private Result ValidateReschedule()
        {
            if (Status == AppointmentStatus.Cancelled || Status == AppointmentStatus.Completed)
                return Result.Failure(
                    new DomainError("NotConfirmed", "Apenas agendamentos ativos podem ser reagendados"));

            return Result.Success();
        }
        
        #endregion
    }
}
