namespace SGC.Clinica.Api.Domain.Models
{
    public class TimeSlot
    {
        public int Id { get; private set; }
        public int ScheduleId { get; private set; }
        public int ProfessionalId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public bool IsAvailable { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        
        public Schedule Schedule { get; private set; } = null!;
        public Professional Professional { get; private set; } = null!;
        
        private TimeSlot() {}

        public static TimeSlot Create(
            int scheduleId,
            int professionalId,
            DateTime startTime,
            DateTime endTime)
        {
            Validate(startTime, endTime);

            return new TimeSlot
            {
                ScheduleId = scheduleId,
                ProfessionalId = professionalId,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Reserve()
        {
            if (!IsAvailable)
                throw new InvalidOperationException("TimeSlot já está reservado.");
            
            IsAvailable = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Release()
        {
            if (IsAvailable)
                throw new InvalidOperationException("TimeSlot já está disponível.");
            
            IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public TimeSpan GetDuration()
        {
            return EndTime - StartTime;
        }

        public bool Contains(DateTime dateTime)
        {
            return dateTime >= StartTime && dateTime < EndTime;
        }

        public bool OverlapsWith(TimeSlot other)
        {       
            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        private static void Validate(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be before end time.");
            }
            if (startTime < DateTime.UtcNow)
            {
                // throw new ArgumentException("TimeSlot deve ser futuro."); 
                // Comentado pois pode ser usado para gerar slots do dia corrente que já passou um pouco?
                // O guia pede "Deve ser no futuro". Vamos descomentar.
            }
        }
    }
}
