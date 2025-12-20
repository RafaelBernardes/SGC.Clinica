namespace SGC.Clinica.Api.Application.Appointment.Dtos
{
    public class CreateAppointmentDto
    {
        public int PatientId { get; set; }
        public int ProfessionalId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
    }
}
