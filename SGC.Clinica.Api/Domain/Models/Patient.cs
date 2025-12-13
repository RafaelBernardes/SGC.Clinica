namespace SGC.Clinica.Api.Domain.Models
{
    public class Patient
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime DateOfBirth { get; private set; }
        public string? Email { get; private set; }
        public string Phone { get; private set; } = string.Empty;
        public string Document { get; private set; } = string.Empty; // CPF, para contratos e identificação
        public string? Occupation { get; private set; }
        public string? Observations { get; private set; }
        public bool Active { get; private set; }
        public bool HasWhatsAppOptIn { get; private set; }
        public bool HasSmsOptIn { get; private set; }
        public bool HasEmailOptIn { get; private set; }

        private Patient(){}

        public static Patient Create(
            string name,
            DateTime dateOfBirth,
            string phone,
            string document,
            string? email = null,
            string? occupation = null,
            string? observations = null)
        {
            // TODO: Adicionar validações (Guard Clauses) para garantir a consistência dos dados.
            // Ex: Validar se o nome não é nulo/vazio, se o documento é um CPF válido, etc.
            return new Patient
            {
                Name = name,
                DateOfBirth = dateOfBirth,
                Phone = phone,
                Document = document,
                Email = email,
                Occupation = occupation,
                Observations = observations,
                Active = true,
                HasWhatsAppOptIn = true, 
                HasSmsOptIn = true,
                HasEmailOptIn = true
            };
        }

        public void Update(
            string name,
            DateTime dateOfBirth,
            string phone,
            string? email,
            string? occupation,
            string? observations)
        {
            // TODO: Adicionar validações (Guard Clauses).
            Name = name;
            DateOfBirth = dateOfBirth;
            Phone = phone;
            Email = email;
            Occupation = occupation;
            Observations = observations;
        }

        public void UpdateConsent(bool hasWhatsAppOptIn, bool hasSmsOptIn, bool hasEmailOptIn)
        {
            HasWhatsAppOptIn = hasWhatsAppOptIn;
            HasSmsOptIn = hasSmsOptIn;
            HasEmailOptIn = hasEmailOptIn;
        }

        public void Deactivate()
        {
            //TODO: Add validations
            Active = false;
        }
    }
}