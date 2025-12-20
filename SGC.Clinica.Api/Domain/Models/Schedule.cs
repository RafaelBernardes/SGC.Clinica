namespace SGC.Clinica.Api.Domain.Models
{
    public class Schedule
    {
        public int Id { get; private set; }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public int ProfessionalId { get; private set; }
        public bool IsAvailable { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    
        public Professional Professional { get; private set; } = null!;

        private Schedule() {}

        public static Schedule Create(
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            DayOfWeek DayOfWeek,
            int professionalId)
        {
            ValidateTimespan(startTime, endTime);

            return new Schedule
            {
                DayOfWeek = DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                ProfessionalId = professionalId,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public void Update(
            TimeSpan startTime,
            TimeSpan endTime,
            bool isAvailable)
        {
            ValidateTimespan(startTime, endTime);

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

        #region Validations

        private static void ValidateTimespan(TimeSpan startTime, TimeSpan endTime)
        {
            if (!(startTime <= endTime))
            {
                throw new ArgumentException("Start time must be before end time.");
            }
        }

        #endregion
    }
}