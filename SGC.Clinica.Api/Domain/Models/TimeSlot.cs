namespace SGC.Clinica.Api.Domain.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int ProfessionalId { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; } = null!;
        public Professional Professional { get; set; } = null!;
        
        private TimeSlot() {}

        public static TimeSlot Create(
            DateTime startTime,
            DateTime endTime,
            int professionalId,
            int scheduleId)
        {
            ValidateTimeSlot(startTime, endTime);

            return new TimeSlot
            {
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = true,
                ProfessionalId = professionalId,
                ScheduleId = scheduleId
            };
        }

        public void Reserve()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("TimeSlot já está reservado.");
        
        IsAvailable = false;
    }

    public void Release()
    {
        if (IsAvailable)
            throw new InvalidOperationException("TimeSlot já está disponível.");
        
        IsAvailable = true;
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
        return StartTime < other.EndTime || other.StartTime < EndTime;
    }

        #region Validations

        private static void ValidateTimeSlot(DateTime startTime, DateTime endTime)
        {
            if (!(startTime <= endTime))
            {
                throw new ArgumentException("Start time must be before end time.");
            }
        }

        #endregion
    }
}