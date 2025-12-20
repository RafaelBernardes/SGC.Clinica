using SGC.Clinica.Api.Application.Enum;

namespace SGC.Clinica.Api.Domain.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int PatientId { get; set; }
        public int ProfessionalId { get; set; }
        public AppointmentStatusEnum Status { get; set; }
        public int DurationInMinutes { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        public Professional Professional { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;

        private Appointment() {}

        public static Appointment Create(
            DateTime scheduledDate,
            int patientId,
            int professionalId,
            AppointmentStatusEnum status,
            int durationInMinutes,
            string notes)
        {
            ValidateScheduledDate(scheduledDate);
            ValidateIds(patientId, professionalId);
        
            return new Appointment
            {
                ScheduledDate = scheduledDate,
                PatientId = patientId,
                ProfessionalId = professionalId,
                Status = AppointmentStatusEnum.Pending,
                DurationInMinutes = durationInMinutes,
                Notes = notes
            };
        }

        public void Confirm()
        {
            Status = AppointmentStatusEnum.Confirmed;
        }

        public void Cancel(string reason)
        {
            Status = AppointmentStatusEnum.Cancelled;
            Reason = reason;
        }

        public void Complete()
        {
            ValidateCanComplete(Status);
            Status = AppointmentStatusEnum.Completed;
        }

        public void Reschedule(DateTime newScheduledDate)
        {
            ValidateScheduledDate(newScheduledDate);
            ScheduledDate = newScheduledDate;
            Status = AppointmentStatusEnum.Rescheduled;
        }

        public void MarkAsNoShow()
        {
            Status = AppointmentStatusEnum.NoShow;
        }


        #region Validations

        private static void ValidateScheduledDate(DateTime scheduledDate)
        {
            if (DateTime.UtcNow > scheduledDate)
                throw new ArgumentException("Data deve estar no futuro.");
        }

        private static void ValidateIds(int patientId, int professionalId)
        {
            if (patientId <= 0)
                throw new ArgumentException("PatientId inválido.");
            if (professionalId <= 0)
                throw new ArgumentException("ProfessionalId inválido.");
        }

        private static void ValidateCanComplete(AppointmentStatusEnum status)
        {
            if (status != AppointmentStatusEnum.Pending && 
                status != AppointmentStatusEnum.Confirmed)
            {
                throw new InvalidOperationException("Apenas compromissos confirmados podem ser concluídos.");
    }
            }

        #endregion
    }
}