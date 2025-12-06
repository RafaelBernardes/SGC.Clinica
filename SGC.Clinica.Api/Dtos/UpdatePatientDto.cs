namespace SGC.Clinica.Api.Dtos
{
    public class UpdatePatientDto
    {
        public string Name { get; set; } = string.Empty;
        
        public DateTime DateOfBirth { get; set; }
    }
}