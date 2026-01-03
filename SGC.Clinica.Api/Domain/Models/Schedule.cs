namespace SGC.Clinica.Api.Domain.Models
{
    public class Schedule
    {
        public int Id { get; private set; }
        public int ProfessionalId { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool IsAvailable { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
    
        public Professional Professional { get; private set; } = null!;

        private Schedule() {}

        public static Schedule Create(
            int professionalId,
            DayOfWeek dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime)
        {
            Validate(startTime, endTime);

            return new Schedule
            {
                ProfessionalId = professionalId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void Update(
            TimeSpan startTime,
            TimeSpan endTime,
            bool isAvailable)
        {
            Validate(startTime, endTime);

            StartTime = startTime;
            EndTime = endTime;
            IsAvailable = isAvailable;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAvailable()
        {
            IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUnavailable()
        {
            IsAvailable = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public TimeSpan GetDuration()
        {
            return EndTime - StartTime;
        }

        private static void Validate(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("Horário de início deve ser anterior ao fim.");
            }
            if (startTime.TotalHours < 0 || endTime.TotalHours > 24)
            {
                 throw new ArgumentException("Horários inválidos.");
            }
        }
    }
}
