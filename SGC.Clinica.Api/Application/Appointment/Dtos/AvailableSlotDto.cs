namespace SGC.Clinica.Api.Application.Appointment.Dtos
{
    public class AvailableSlotDto
    {
        public int SlotId { get; set; }
        public int ProfessionalId { get; set; }
        public string? ProfessionalName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
