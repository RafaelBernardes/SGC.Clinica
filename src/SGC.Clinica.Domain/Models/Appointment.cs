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

        public static Result Create(
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
                return Result.Failure([.. validation.Errors]);

            var appointment = new Appointment(patientId, professionalId, scheduledDate, duration, notes);

            appointment.AddDomainEvent(new AppointmentScheduledEvent(patient.Id, patient.Name, patient.Email));
            
            return Result<Patient>.Success(patient);
        }

        public void Confirm()
        {
            if (Status == AppointmentStatus.Pending)
            {
                Status = AppointmentStatus.Confirmed;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Motivo do cancelamento é obrigatório.");

            Status = AppointmentStatus.Cancelled;
            CancellationReason = reason;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Apenas agendamentos confirmados podem ser concluídos.");

            Status = AppointmentStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reschedule(DateTime newDate)
        {
            if (newDate <= DateTime.UtcNow)
                throw new ArgumentException("Nova data deve ser futura.");

            ScheduledDate = newDate;
            Status = AppointmentStatus.Rescheduled;
            UpdatedAt = DateTime.UtcNow;
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

        private static Result ValidateDeactivation(bool isActive)
        {
            if (!isActive)
                return Result.Failure(new ValidationError("PatientInactive", "Paciente já está inativo"));

            return Result.Success();
        }
        
        #endregion
    }
}
