namespace SGC.Clinica.Api.Application.Schedules.Dtos
{
    public class CreateScheduleDto
    {
        public int ProfessionalId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
