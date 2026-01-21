
namespace SGC.Clinica.Api.Application.Patients.Dtos
{
    public class AddPatientDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}
