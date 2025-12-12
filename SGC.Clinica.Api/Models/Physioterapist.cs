namespace SGC.Clinica.Api.Models
{
    public class Physioterapist : Professional
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;

        private Physioterapist(string name, string document, string phone, string licenseNumber, string specialty) 
            : base(name, document, phone)
        {
            LicenseNumber = licenseNumber;
            Specialty = specialty;
        }

        public static Physioterapist Create(string name, string document, string phone, string licenseNumber, string specialty)
        {
            return new Physioterapist(name, document, phone, licenseNumber, specialty);
        }
    }
}