using SGC.Clinica.Api.Domain.Enums;
using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Domain.Events.Appointments;

namespace SGC.Clinica.Api.Domain.Models
{
    public class Appointment
    {
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
        
        // Propriedades de navegação
        public Patient Patient { get; private set; } = null!;
        public Professional Professional { get; private set; } = null!;

        private Appointment() { }

        public static Appointment Create(
            int patientId,
            int professionalId,
            DateTime scheduledDate,
            TimeSpan duration,
            string notes)
        {
            Validate(patientId, professionalId, scheduledDate, duration);

            var appointment = new Appointment
            {
                PatientId = patientId,
                ProfessionalId = professionalId,
                ScheduledDate = scheduledDate,
                Duration = duration,
                Notes = notes ?? string.Empty,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            
            return appointment;
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

        private static void Validate(int patientId, int professionalId, DateTime scheduledDate, TimeSpan duration)
        {
            if (patientId <= 0) throw new ArgumentException("PatientId inválido.");
            if (professionalId <= 0) throw new ArgumentException("ProfessionalId inválido.");
            if (scheduledDate <= DateTime.UtcNow) throw new ArgumentException("Data deve ser futura.");
            if (duration.TotalMinutes < 15 || duration.TotalMinutes > 480) 
                throw new ArgumentException("Duração deve ser entre 15 e 480 minutos.");
        }
    }
}
