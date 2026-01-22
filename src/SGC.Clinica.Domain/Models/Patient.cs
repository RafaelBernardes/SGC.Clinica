using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Domain.Events.Patients;
using SGC.Clinica.Api.Domain.Helpers;
using SGC.Clinica.Domain.Results;

namespace SGC.Clinica.Api.Domain.Models
{
    public sealed class Patient
    {
        // Private constructor - Force factory method
        private Patient(
            string name,
            DateTime dateOfBirth,
            string phone,
            string document,
            string email,
            string? occupation = null,
            string? observations = null)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            Phone = phone;
            Document = document;
            Email = email;
            Occupation = occupation;
            Observations = observations;
            Active = true;
            HasWhatsAppOptIn = true;
            HasSmsOptIn = true;
            HasEmailOptIn = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Document { get; private set; }
        public string? Occupation { get; private set; }
        public string? Observations { get; private set; }
        public bool Active { get; private set; }
        public bool HasWhatsAppOptIn { get; private set; }
        public bool HasSmsOptIn { get; private set; }
        public bool HasEmailOptIn { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<IDomainEvent> _domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public static Result<Patient> Create(
            string name,
            DateTime dateOfBirth,
            string phone,
            string document,
            string email,
            string? occupation = null,
            string? observations = null)
        {
            var validation = ValidationHelper.Validate(
                ValidateName(name),
                ValidateDateOfBirth(dateOfBirth),
                ValidatePhone(phone),
                ValidateDocument(document), 
                ValidateEmail(email)
            );

            if (validation.IsFailure)
                return Result<Patient>.Failure([.. validation.Errors]);

            var patient = new Patient(name, dateOfBirth, phone, document, email, occupation, observations);

            patient.AddDomainEvent(new PatientCreatedEvent(patient.Id, patient.Name, patient.Email));
            
            return Result<Patient>.Success(patient);
        }

            // ✅ Comportamentos da entidade
        public Result Update(
            string name,
            DateTime dateOfBirth,
            string phone,
            string email,
            string? occupation,
            string? observations)
        {
            var validation = ValidationHelper.Validate(
                ValidateName(name),
                ValidateDateOfBirth(dateOfBirth),
                ValidatePhone(phone),
                ValidateEmail(email)
            );

            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            Name = name;
            DateOfBirth = dateOfBirth;
            Phone = phone;
            Email = email;
            Occupation = occupation;
            Observations = observations;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientUpdatedEvent(Id, Name, Email));

            return Result.Success();
        }

        public Result Deactivate()
        {
            var validation = ValidateDeactivation();
            if (validation.IsFailure)
                return Result.Failure([.. validation.Errors]);

            Active = false;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new PatientDeletedEvent(Id, Name, Email));

            return Result.Success();
        }

        #region Events

        public void ClearDomainEvents() => _domainEvents.Clear();

        private void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        #endregion

        #region guard clauses

        private static Result ValidateName(string name)
        {
            var errors = new List<IError>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add(new ValidationError("NameRequired", "Nome é obrigatório"));

            if (!string.IsNullOrWhiteSpace(name) && (name.Length < 2 || name.Length > 150))
                errors.Add(new ValidationError("NameLength", "Nome deve ter entre 2 e 150 caracteres"));

            if (!string.IsNullOrWhiteSpace(name) && !IsValidNameFormat(name))
                errors.Add(new ValidationError("NameFormat", "Nome não pode conter números ou caracteres especiais"));

            return errors.Any() 
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateDocument(string document)
        {
            var errors = new List<IError>();

            if (string.IsNullOrWhiteSpace(document))
                errors.Add(new ValidationError("DocumentRequired", "Documento é obrigatório"));

            if (!string.IsNullOrWhiteSpace(document) && !IsValidCPF(document))
                errors.Add(new ValidationError("InvalidCPF", "CPF inválido"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidatePhone(string phone)
        {
            var errors = new List<IError>();

            if (string.IsNullOrWhiteSpace(phone))
                errors.Add(new ValidationError("PhoneRequired", "Telefone é obrigatório"));

            if (!string.IsNullOrWhiteSpace(phone) && !IsValidPhoneFormat(phone))
                errors.Add(new ValidationError("PhoneFormat", "Formato de telefone inválido"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateDateOfBirth(DateTime dateOfBirth)
        {
            var errors = new List<IError>();

            if (dateOfBirth >= DateTime.Today)
                errors.Add(new ValidationError("DOBFuture", "Data de nascimento deve ser no passado"));

            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (age < 0 || age > 150)
                errors.Add(new ValidationError("InvalidAge", "Idade deve estar entre 0 e 150 anos"));

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private static Result ValidateEmail(string email)
        {
            var errors = new List<IError>();

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                    errors.Add(new ValidationError("EmailFormat", "Email inválido"));
            }
            catch
            {
                errors.Add(new ValidationError("EmailFormat", "Email inválido"));
            }

            return errors.Any()
                ? Result.Failure(errors.ToArray())
                : Result.Success();
        }

        private Result ValidateDeactivation()
        {
            if (!Active)
                return Result.Failure(new ValidationError("PatientInactive", "Paciente já está inativo"));

            return Result.Success();
        }
        
        private static bool IsValidNameFormat(string name) =>
            System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-ZÀ-ÿ\s]*$");

        private static bool IsValidPhoneFormat(string phone) =>
            System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\(?[1-9]{2}\)?\s?9?\d{4}-?\d{4}$");

        private static bool IsValidCPF(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                return false;

            // Validar dígitos verificadores
            int CalculateDigit(string value, int length)
            {
                int sum = 0;
                for (int i = 0; i < length; i++)
                    sum += (value[i] - '0') * (length + 1 - i);

                int digit = 11 - (sum % 11);
                return digit > 9 ? 0 : digit;
            }

            if (CalculateDigit(cpf, 9) != cpf[9] - '0')
                return false;

            return CalculateDigit(cpf, 10) == cpf[10] - '0';
        }
        
        #endregion
    }
}