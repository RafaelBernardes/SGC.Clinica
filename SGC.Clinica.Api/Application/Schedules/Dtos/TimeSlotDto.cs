namespace SGC.Clinica.Api.Application.Schedules.Dtos
{
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int ProfessionalId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
