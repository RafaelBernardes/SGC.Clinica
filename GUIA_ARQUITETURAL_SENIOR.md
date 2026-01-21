# 🏗️ Guia Arquitetural Sênior - SGC.Clínica
## Refatoração Completa + Implementação Fase 2.1

**Data:** Janeiro 2026  
**Padrão:** Clean Architecture + DDD + CQRS + Event-Driven  
**Nível:** 🥈 Senior

---

## 📋 Índice

1. [Fundação Arquitetural](#1-fundação-arquitetural)
2. [Estrutura de Projetos](#2-estrutura-de-projetos)
3. [Padrões & Decisões](#3-padrões--decisões)
4. [Refatoração Fase 0](#4-refatoração-fase-0)
5. [Implementação Fase 2.1](#5-implementação-fase-21)
6. [Infraestrutura & Observabilidade](#6-infraestrutura--observabilidade)
7. [Testes](#7-testes)
8. [Checklist Final](#8-checklist-final)

---

# 1. FUNDAÇÃO ARQUITETURAL

## 1.1 Pilares da Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                  Presentation (HTTP)                         │
│                                                              │
│    Controllers → MediatR Pipeline → Handlers                │
└─────────────────────┬───────────────────────────────────────┘
                      │ Abstração via Interfaces
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                          │
│                                                              │
│  Commands/Queries → Validators → Handlers                  │
│                                  ├─> Business Logic         │
│                                  ├─> Events Publishing      │
│                                  └─> Result Pattern         │
└─────────────────────┬───────────────────────────────────────┘
                      │ Abstração via IUnitOfWork, Specifications
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                              │
│                                                              │
│  Models → Guard Clauses (Result<T>) → Invariantes          │
│  Value Objects → Domain Events                             │
└─────────────────────┬───────────────────────────────────────┘
                      │ Abstração via Interfaces
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                         │
│                                                              │
│  ├─ Persistence (EF Core, Repositories, UnitOfWork)        │
│  ├─ Services (Email, SMS, Notification)                    │
│  ├─ Events (RabbitMQ Publisher/Consumer)                   │
│  ├─ Logging (Serilog)                                      │
│  └─ Authentication (JWT, RBAC)                             │
└─────────────────────────────────────────────────────────────┘
```

## 1.2 Padrões Escolhidos

### ✅ Result Pattern
```csharp
// Domain sempre retorna Result<T>
public static Result<Patient> Create(string name, ...)
{
    // Guard Clauses retornam Result.Failure
    if (string.IsNullOrEmpty(name))
        return Result<Patient>.Failure(new ValidationError("Name is required"));
    
    // Success
    return Result<Patient>.Success(new Patient { ... });
}

// Application consome Result<T>
if (!result.IsSuccess)
    return BadRequest(result.Errors);

var patient = result.Value;
```

### ✅ CQRS (Commands & Queries Separados)
```csharp
// Commands = alteração de estado
public class CreatePatientCommand : IRequest<Result<PatientDto>>

// Queries = leitura (sem efeito colateral)
public class GetPatientByIdQuery : IRequest<Result<PatientDto>>
```

### ✅ Domain Events (para RabbitMQ)
```csharp
// Quando paciente é criado, evento é publicado
domain.AddDomainEvent(new PatientCreatedEvent(patient.Id, patient.Email));

// Async via RabbitMQ
// Handler publica para fila: send welcome email, create user profile, etc
```

### ✅ Repository Pattern (Hybrid)
```csharp
// Generic para operações padrão
IRepository<Patient> patientRepo = ...
var patient = await patientRepo.GetByIdAsync(id);

// Specific para queries complexas
IPatientRepository patientRepo = ...
var activePatients = await patientRepo.GetActiveWithAppointmentsAsync(startDate);
```

---

# 2. ESTRUTURA DE PROJETOS

## 2.1 Nova Estrutura (Múltiplos .csproj)

```
SGC.Clinica/
├── src/
│   ├── SGC.Clinica.Domain/
│   │   ├── Models/
│   │   │   ├── Patient.cs
│   │   │   ├── Professional.cs
│   │   │   ├── Appointment.cs
│   │   │   ├── Schedule.cs
│   │   │   └── TimeSlot.cs
│   │   ├── Events/
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── PatientEvents.cs
│   │   │   └── AppointmentEvents.cs
│   │   ├── Enums/
│   │   │   └── AppointmentStatus.cs
│   │   ├── ValueObjects/
│   │   │   ├── Email.cs
│   │   │   ├── PhoneNumber.cs
│   │   │   └── CPF.cs
│   │   ├── Exceptions/
│   │   │   └── DomainException.cs
│   │   ├── Results/
│   │   │   ├── Result.cs
│   │   │   ├── ValidationError.cs
│   │   │   └── DomainError.cs
│   │   └── SGC.Clinica.Domain.csproj
│   │
│   ├── SGC.Clinica.Application/
│   │   ├── Abstractions/
│   │   │   ├── IUnitOfWork.cs
│   │   │   ├── IRepository.cs
│   │   │   ├── IEmailService.cs
│   │   │   └── IEventPublisher.cs
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── TransactionBehavior.cs
│   │   ├── Patients/
│   │   │   ├── Commands/
│   │   │   │   ├── CreatePatientCommand.cs
│   │   │   │   ├── UpdatePatientCommand.cs
│   │   │   │   └── DeletePatientCommand.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetPatientByIdQuery.cs
│   │   │   │   └── GetPatientsListQuery.cs
│   │   │   ├── Handlers/
│   │   │   │   ├── CreatePatientCommandHandler.cs
│   │   │   │   ├── UpdatePatientCommandHandler.cs
│   │   │   │   ├── DeletePatientCommandHandler.cs
│   │   │   │   ├── GetPatientByIdQueryHandler.cs
│   │   │   │   └── GetPatientsListQueryHandler.cs
│   │   │   ├── Validators/
│   │   │   │   ├── CreatePatientCommandValidator.cs
│   │   │   │   ├── UpdatePatientCommandValidator.cs
│   │   │   │   └── DeletePatientCommandValidator.cs
│   │   │   ├── Dtos/
│   │   │   │   ├── PatientDto.cs
│   │   │   │   ├── CreatePatientDto.cs
│   │   │   │   └── UpdatePatientDto.cs
│   │   │   └── EventHandlers/
│   │   │       └── PatientCreatedEventHandler.cs
│   │   ├── Schedules/
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   ├── Handlers/
│   │   │   ├── Validators/
│   │   │   ├── Dtos/
│   │   │   └── EventHandlers/
│   │   ├── Appointments/
│   │   │   └── (similar to Schedules)
│   │   ├── Mappings/
│   │   │   ├── PatientMappingProfile.cs
│   │   │   ├── ScheduleMappingProfile.cs
│   │   │   └── AppointmentMappingProfile.cs
│   │   └── SGC.Clinica.Application.csproj
│   │
│   ├── SGC.Clinica.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Data/
│   │   │   │   ├── AppDbContext.cs
│   │   │   │   ├── Configurations/
│   │   │   │   │   ├── PatientConfiguration.cs
│   │   │   │   │   ├── ProfessionalConfiguration.cs
│   │   │   │   │   ├── AppointmentConfiguration.cs
│   │   │   │   │   ├── ScheduleConfiguration.cs
│   │   │   │   │   └── TimeSlotConfiguration.cs
│   │   │   │   ├── Interfaces/
│   │   │   │   │   └── IApplicationDbContext.cs
│   │   │   │   └── Migrations/
│   │   │   ├── Repositories/
│   │   │   │   ├── Base/
│   │   │   │   │   ├── Repository.cs
│   │   │   │   │   ├── IRepository.cs
│   │   │   │   │   └── Specifications/
│   │   │   │   │       ├── Specification.cs
│   │   │   │   │       ├── SpecificationEvaluator.cs
│   │   │   │   │       └── ISpecification.cs
│   │   │   │   ├── Patient/
│   │   │   │   │   ├── IPatientRepository.cs
│   │   │   │   │   └── PatientRepository.cs
│   │   │   │   ├── Schedule/
│   │   │   │   ├── Appointment/
│   │   │   │   └── UnitOfWork/
│   │   │   │       ├── IUnitOfWork.cs
│   │   │   │       └── UnitOfWork.cs
│   │   ├── Services/
│   │   │   ├── Email/
│   │   │   │   ├── IEmailService.cs
│   │   │   │   └── EmailService.cs
│   │   │   ├── Sms/
│   │   │   ├── Notification/
│   │   │   └── Authentication/
│   │   │       ├── IJwtTokenProvider.cs
│   │   │       └── JwtTokenProvider.cs
│   │   ├── Events/
│   │   │   ├── RabbitMQ/
│   │   │   │   ├── RabbitMqPublisher.cs
│   │   │   │   ├── RabbitMqConsumer.cs
│   │   │   │   └── RabbitMqSettings.cs
│   │   │   └── Handlers/
│   │   │       ├── PatientCreatedEventHandler.cs
│   │   │       └── AppointmentScheduledEventHandler.cs
│   │   ├── Logging/
│   │   │   ├── LoggingExtensions.cs
│   │   │   └── SerilogConfiguration.cs
│   │   └── SGC.Clinica.Infrastructure.csproj
│   │
│   └── SGC.Clinica.API/
│       ├── Controllers/
│       │   ├── PatientsController.cs
│       │   ├── SchedulesController.cs
│       │   ├── AppointmentsController.cs
│       │   └── AuthController.cs
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   ├── LoggingMiddleware.cs
│       │   └── AuthenticationMiddleware.cs
│       ├── Filters/
│       │   └── ValidationActionFilter.cs
│       ├── DependencyInjection/
│       │   ├── ServiceCollectionExtensions.cs
│       │   └── ServiceConfigurations.cs
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── SGC.Clinica.API.csproj
│
├── tests/
│   ├── SGC.Clinica.Domain.Tests/
│   │   ├── Models/
│   │   │   ├── PatientTests.cs
│   │   │   ├── AppointmentTests.cs
│   │   │   └── ScheduleTests.cs
│   │   └── SGC.Clinica.Domain.Tests.csproj
│   │
│   ├── SGC.Clinica.Application.Tests/
│   │   ├── Patients/
│   │   │   ├── CreatePatientCommandHandlerTests.cs
│   │   │   ├── CreatePatientCommandValidatorTests.cs
│   │   │   └── GetPatientByIdQueryHandlerTests.cs
│   │   ├── Schedules/
│   │   └── SGC.Clinica.Application.Tests.csproj
│   │
│   └── SGC.Clinica.API.Tests/
│       ├── Integration/
│       │   ├── Patients/
│       │   │   ├── CreatePatientTests.cs
│       │   │   └── UpdatePatientTests.cs
│       │   └── Schedules/
│       ├── SGC.Clinica.API.Tests.csproj
│       └── TestFixtures/
│           ├── DatabaseFixture.cs
│           └── RabbitMqFixture.cs
│
├── SGC.Clinica.sln
├── docker-compose.yml
└── README.md
```

## 2.2 Configuração do .sln

```xml
<!-- SGC.Clinica.sln -->
Microsoft Visual Studio Solution File, Format Version 12.00

Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.Domain", "src\SGC.Clinica.Domain\SGC.Clinica.Domain.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.Application", "src\SGC.Clinica.Application\SGC.Clinica.Application.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.Infrastructure", "src\SGC.Clinica.Infrastructure\SGC.Clinica.Infrastructure.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.API", "src\SGC.Clinica.API\SGC.Clinica.API.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.Domain.Tests", "tests\SGC.Clinica.Domain.Tests\SGC.Clinica.Domain.Tests.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.Application.Tests", "tests\SGC.Clinica.Application.Tests\SGC.Clinica.Application.Tests.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SGC.Clinica.API.Tests", "tests\SGC.Clinica.API.Tests\SGC.Clinica.API.Tests.csproj", "{...}"
EndProject
```

---

# 3. PADRÕES & DECISÕES

## 3.1 Result Pattern Detalhado

### Domain.Results

```csharp
// SGC.Clinica.Domain/Results/Result.cs
public abstract record Result(bool IsSuccess, IEnumerable<IError> Errors);

public sealed record ResultSuccess : Result
{
    public ResultSuccess() : base(true, Enumerable.Empty<IError>()) { }
}

public sealed record ResultFailure : Result
{
    public ResultFailure(IEnumerable<IError> errors) : base(false, errors) { }
    public ResultFailure(params IError[] errors) : base(false, errors.AsEnumerable()) { }
    public ResultFailure(IError error) : base(false, new[] { error }) { }
}

public abstract record Result<T>(bool IsSuccess, T? Value, IEnumerable<IError> Errors)
{
    public sealed record Success(T Value) : Result<T>(true, Value, Enumerable.Empty<IError>)
    {
        public Success(T value) : this(value) { }
    }

    public sealed record Failure(IEnumerable<IError> Errors) : Result<T>(false, null, Errors)
    {
        public Failure(params IError[] errors) : this(errors.AsEnumerable()) { }
        public Failure(IError error) : this(new[] { error }) { }
    }

    public static Result<T> Success(T value) => new Success(value);
    public static Result<T> Failure(IError error) => new Failure(error);
    public static Result<T> Failure(params IError[] errors) => new Failure(errors);

    // Extensões úteis
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<IEnumerable<IError>, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Errors);

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<IEnumerable<IError>, Task<TResult>> onFailure) =>
        IsSuccess ? await onSuccess(Value!) : await onFailure(Errors);
}
```

### Domain.Results/Errors

```csharp
// SGC.Clinica.Domain/Results/IError.cs
public interface IError
{
    string Code { get; }
    string Message { get; }
    ErrorType Type { get; }
}

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    InternalServer
}

// SGC.Clinica.Domain/Results/ValidationError.cs
public sealed record ValidationError(string Code, string Message) : IError
{
    public ErrorType Type => ErrorType.Validation;

    public ValidationError(string message) : this(nameof(ValidationError), message) { }
}

// SGC.Clinica.Domain/Results/DomainError.cs
public sealed record DomainError(string Code, string Message) : IError
{
    public ErrorType Type => ErrorType.Conflict;

    public DomainError(string message) : this(nameof(DomainError), message) { }
}

// SGC.Clinica.Domain/Results/NotFoundError.cs
public sealed record NotFoundError(string Code, string Message) : IError
{
    public ErrorType Type => ErrorType.NotFound;

    public NotFoundError(string resourceName, object key) 
        : this($"{resourceName}NotFound", $"{resourceName} with id {key} was not found") { }
}
```

## 3.2 Guard Clauses com Result Pattern

```csharp
// SGC.Clinica.Domain/Models/Patient.cs
public sealed class Patient
{
    // Private constructor - Force factory method
    private Patient(
        string name,
        DateTime dateOfBirth,
        string phone,
        string document,
        string? email = null,
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
    public string? Email { get; private set; }
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

    // ✅ Guard Clauses via Result Pattern
    public static Result<Patient> Create(
        string name,
        DateTime dateOfBirth,
        string phone,
        string document,
        string? email = null,
        string? occupation = null,
        string? observations = null)
    {
        // Validar cada campo com guard clause
        var nameValidation = ValidateName(name);
        if (!nameValidation.IsSuccess)
            return nameValidation.Match(
                onSuccess: _ => Result<Patient>.Success(null!),
                onFailure: errors => Result<Patient>.Failure(errors.ToArray())
            );

        var documentValidation = ValidateDocument(document);
        if (!documentValidation.IsSuccess)
            return documentValidation.Match(
                onSuccess: _ => Result<Patient>.Success(null!),
                onFailure: errors => Result<Patient>.Failure(errors.ToArray())
            );

        var phoneValidation = ValidatePhone(phone);
        if (!phoneValidation.IsSuccess)
            return phoneValidation.Match(
                onSuccess: _ => Result<Patient>.Success(null!),
                onFailure: errors => Result<Patient>.Failure(errors.ToArray())
            );

        var dobValidation = ValidateDateOfBirth(dateOfBirth);
        if (!dobValidation.IsSuccess)
            return dobValidation.Match(
                onSuccess: _ => Result<Patient>.Success(null!),
                onFailure: errors => Result<Patient>.Failure(errors.ToArray())
            );

        if (!string.IsNullOrEmpty(email))
        {
            var emailValidation = ValidateEmail(email);
            if (!emailValidation.IsSuccess)
                return emailValidation.Match(
                    onSuccess: _ => Result<Patient>.Success(null!),
                    onFailure: errors => Result<Patient>.Failure(errors.ToArray())
                );
        }

        // Tudo validado - criar patient
        var patient = new Patient(name, dateOfBirth, phone, document, email, occupation, observations);
        
        // Publicar evento de domínio
        patient._domainEvents.Add(
            new PatientCreatedEvent(patient.Id, patient.Name, patient.Email));

        return Result<Patient>.Success(patient);
    }

    // ✅ Validadores privados
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
            : Result.Success(true);
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
            : Result.Success(true);
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
            : Result.Success(true);
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
            : Result.Success(true);
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
            : Result.Success(true);
    }

    // ✅ Validadores auxiliares
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

    // ✅ Comportamentos da entidade
    public void Update(
        string name,
        DateTime dateOfBirth,
        string phone,
        string? email,
        string? occupation,
        string? observations)
    {
        Name = name;
        DateOfBirth = dateOfBirth;
        Phone = phone;
        Email = email;
        Occupation = occupation;
        Observations = observations;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Active = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
```

---

# 4. REFATORAÇÃO FASE 0

## 4.1 Passo a Passo

### 4.1.1 Criar Projetos Separados

```bash
# Criar solução
dotnet new sln -n SGC.Clinica
cd SGC.Clinica

# Criar projetos
mkdir src tests

# Domain
dotnet new classlib -n SGC.Clinica.Domain -o src/SGC.Clinica.Domain
dotnet sln add src/SGC.Clinica.Domain/SGC.Clinica.Domain.csproj

# Application
dotnet new classlib -n SGC.Clinica.Application -o src/SGC.Clinica.Application
dotnet sln add src/SGC.Clinica.Application/SGC.Clinica.Application.csproj

# Infrastructure
dotnet new classlib -n SGC.Clinica.Infrastructure -o src/SGC.Clinica.Infrastructure
dotnet sln add src/SGC.Clinica.Infrastructure/SGC.Clinica.Infrastructure.csproj

# API
dotnet new webapi -n SGC.Clinica.API -o src/SGC.Clinica.API
dotnet sln add src/SGC.Clinica.API/SGC.Clinica.API.csproj

# Tests
dotnet new xunit -n SGC.Clinica.Domain.Tests -o tests/SGC.Clinica.Domain.Tests
dotnet sln add tests/SGC.Clinica.Domain.Tests/SGC.Clinica.Domain.Tests.csproj

dotnet new xunit -n SGC.Clinica.Application.Tests -o tests/SGC.Clinica.Application.Tests
dotnet sln add tests/SGC.Clinica.Application.Tests/SGC.Clinica.Application.Tests.csproj

dotnet new xunit -n SGC.Clinica.API.Tests -o tests/SGC.Clinica.API.Tests
dotnet sln add tests/SGC.Clinica.API.Tests/SGC.Clinica.API.Tests.csproj
```

### 4.1.2 Configurar Dependências Entre Projetos

```bash
# Application depende de Domain
cd src/SGC.Clinica.Application
dotnet add reference ../SGC.Clinica.Domain/SGC.Clinica.Domain.csproj

# Infrastructure depende de Domain e Application
cd ../SGC.Clinica.Infrastructure
dotnet add reference ../SGC.Clinica.Domain/SGC.Clinica.Domain.csproj
dotnet add reference ../SGC.Clinica.Application/SGC.Clinica.Application.csproj

# API depende de todos
cd ../SGC.Clinica.API
dotnet add reference ../SGC.Clinica.Domain/SGC.Clinica.Domain.csproj
dotnet add reference ../SGC.Clinica.Application/SGC.Clinica.Application.csproj
dotnet add reference ../SGC.Clinica.Infrastructure/SGC.Clinica.Infrastructure.csproj

# Tests
cd ../../tests/SGC.Clinica.Domain.Tests
dotnet add reference ../../src/SGC.Clinica.Domain/SGC.Clinica.Domain.csproj
dotnet add package Moq
dotnet add package xunit
dotnet add package xunit.runner.visualstudio

cd ../SGC.Clinica.Application.Tests
dotnet add reference ../../src/SGC.Clinica.Application/SGC.Clinica.Application.csproj
dotnet add reference ../../src/SGC.Clinica.Domain/SGC.Clinica.Domain.csproj
dotnet add package Moq

cd ../SGC.Clinica.API.Tests
dotnet add reference ../../src/SGC.Clinica.API/SGC.Clinica.API.csproj
dotnet add package Moq
```

### 4.1.3 Instalar NuGets Essenciais

```bash
# Domain
cd src/SGC.Clinica.Domain
# Nenhuma dependência externa! (apenas .NET)

# Application
cd ../SGC.Clinica.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Infrastructure
cd ../SGC.Clinica.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package RabbitMQ.Client
dotnet add package Serilog
dotnet add package Serilog.Enrichers.CorrelationId
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.IdentityModel.Tokens

# API
cd ../SGC.Clinica.API
dotnet add package Swashbuckle.AspNetCore
dotnet add package AspNetCore.HealthChecks.SqlServer
dotnet add package AspNetCore.HealthChecks.RabbitMQ
```

### 4.1.4 Estrutura de Namespaces

```csharp
// Domain - SEM dependências externas
namespace SGC.Clinica.Domain.Models { }
namespace SGC.Clinica.Domain.Events { }
namespace SGC.Clinica.Domain.Enums { }
namespace SGC.Clinica.Domain.ValueObjects { }
namespace SGC.Clinica.Domain.Results { }
namespace SGC.Clinica.Domain.Exceptions { }

// Application
namespace SGC.Clinica.Application.Abstractions { }
namespace SGC.Clinica.Application.Behaviors { }
namespace SGC.Clinica.Application.Patients.Commands { }
namespace SGC.Clinica.Application.Patients.Queries { }
namespace SGC.Clinica.Application.Patients.Handlers { }
namespace SGC.Clinica.Application.Patients.Validators { }
namespace SGC.Clinica.Application.Patients.Dtos { }
namespace SGC.Clinica.Application.Patients.EventHandlers { }
// ... Schedules, Appointments

// Infrastructure
namespace SGC.Clinica.Infrastructure.Persistence.Data { }
namespace SGC.Clinica.Infrastructure.Persistence.Repositories { }
namespace SGC.Clinica.Infrastructure.Services { }
namespace SGC.Clinica.Infrastructure.Events { }
namespace SGC.Clinica.Infrastructure.Authentication { }
namespace SGC.Clinica.Infrastructure.Logging { }

// API
namespace SGC.Clinica.API.Controllers { }
namespace SGC.Clinica.API.Middleware { }
namespace SGC.Clinica.API.Filters { }
namespace SGC.Clinica.API.DependencyInjection { }
```

---

# 5. IMPLEMENTAÇÃO FASE 2.1

## 5.1 Domain Layer - Schedules & Appointments

### Appointment.cs com Result Pattern

```csharp
// SGC.Clinica.Domain/Models/Appointment.cs
public sealed class Appointment
{
    private Appointment(
        int patientId,
        int professionalId,
        DateTime scheduledDate,
        TimeSpan duration,
        string notes = "")
    {
        PatientId = patientId;
        ProfessionalId = professionalId;
        ScheduledDate = scheduledDate;
        Duration = duration;
        Notes = notes;
        Status = AppointmentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int ProfessionalId { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Patient Patient { get; private set; } = null!;
    public Professional Professional { get; private set; } = null!;

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // ✅ Factory Method com Guard Clauses
    public static Result<Appointment> Create(
        int patientId,
        int professionalId,
        DateTime scheduledDate,
        TimeSpan duration,
        string notes = "")
    {
        var errors = new List<IError>();

        // Validar IDs
        if (patientId <= 0)
            errors.Add(new ValidationError("InvalidPatientId", "PatientId inválido"));

        if (professionalId <= 0)
            errors.Add(new ValidationError("InvalidProfessionalId", "ProfessionalId inválido"));

        // Validar data
        if (scheduledDate <= DateTime.UtcNow)
            errors.Add(new ValidationError("PastDate", "Data do agendamento deve ser futura"));

        // Validar duração
        if (duration <= TimeSpan.Zero)
            errors.Add(new ValidationError("InvalidDuration", "Duração deve ser positiva"));

        if (duration < TimeSpan.FromMinutes(15) || duration > TimeSpan.FromHours(8))
            errors.Add(new ValidationError("DurationRange", "Duração deve estar entre 15 minutos e 8 horas"));

        if (errors.Any())
            return Result<Appointment>.Failure(errors.ToArray());

        var appointment = new Appointment(patientId, professionalId, scheduledDate, duration, notes);
        
        appointment._domainEvents.Add(
            new AppointmentScheduledEvent(appointment.Id, patientId, professionalId, scheduledDate));

        return Result<Appointment>.Success(appointment);
    }

    // ✅ Comportamentos
    public Result Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            return Result.Failure(
                new DomainError("InvalidStatus", "Apenas agendamentos pendentes podem ser confirmados"));

        Status = AppointmentStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(
            new AppointmentConfirmedEvent(Id, PatientId, ProfessionalId, ScheduledDate));

        return Result.Success(true);
    }

    public Result Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(
                new ValidationError("ReasonRequired", "Motivo do cancelamento é obrigatório"));

        if (Status == AppointmentStatus.Cancelled)
            return Result.Failure(
                new DomainError("AlreadyCancelled", "Agendamento já foi cancelado"));

        if (Status == AppointmentStatus.Completed)
            return Result.Failure(
                new DomainError("AlreadyCompleted", "Agendamento já foi realizado"));

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(
            new AppointmentCancelledEvent(Id, PatientId, CancellationReason));

        return Result.Success(true);
    }

    public Result Complete()
    {
        if (Status != AppointmentStatus.Confirmed)
            return Result.Failure(
                new DomainError("NotConfirmed", "Apenas agendamentos confirmados podem ser completados"));

        Status = AppointmentStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        _domainEvents.Add(
            new AppointmentCompletedEvent(Id, PatientId, ProfessionalId));

        return Result.Success(true);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool HasConflictWith(Appointment other)
    {
        if (other.Status == AppointmentStatus.Cancelled)
            return false;

        if (ProfessionalId != other.ProfessionalId)
            return false;

        var thisEnd = ScheduledDate.Add(Duration);
        var otherEnd = other.ScheduledDate.Add(other.Duration);

        return ScheduledDate < otherEnd && thisEnd > other.ScheduledDate;
    }
}
```

---

# 6. INFRAESTRUTURA & OBSERVABILIDADE

## 6.1 RabbitMQ Event Publisher

```csharp
// SGC.Clinica.Infrastructure/Events/RabbitMQ/RabbitMqPublisher.cs
public class RabbitMqPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly RabbitMqSettings _settings;

    public RabbitMqPublisher(
        IConnection connection,
        ILogger<RabbitMqPublisher> logger,
        RabbitMqSettings settings)
    {
        _connection = connection;
        _logger = logger;
        _settings = settings;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        using var channel = _connection.CreateModel();
        
        var exchangeName = $"{typeof(TEvent).Name}.exchange";
        var routingKey = typeof(TEvent).Name;

        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true);

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        channel.BasicPublish(exchangeName, routingKey, properties, body);

        _logger.LogInformation(
            "Domain event published: {EventType} with routing key: {RoutingKey}",
            typeof(TEvent).Name,
            routingKey);

        await Task.CompletedTask;
    }
}
```

## 6.2 Serilog Configuration

```csharp
// SGC.Clinica.Infrastructure/Logging/LoggingExtensions.cs
public static class LoggingExtensions
{
    public static IHostBuilder UseAdvancedLogging(this IHostBuilder hostBuilder) =>
        hostBuilder.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "SGC.Clinica")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(new CompactJsonTheme())
                .WriteTo.File(
                    path: "logs/sgc-clinica-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        });
}
```

## 6.3 Behaviors para MediatR

```csharp
// SGC.Clinica.Application/Behaviors/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var activity = new System.Diagnostics.Activity($"MediatR.{typeof(TRequest).Name}").Start();
        
        _logger.LogInformation(
            "Executing {RequestName} with {@Request}",
            typeof(TRequest).Name,
            request);

        try
        {
            var response = await next();
            _logger.LogInformation("Successfully executed {RequestName}", typeof(TRequest).Name);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}

// SGC.Clinica.Application/Behaviors/TransactionBehavior.cs
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}", typeof(TRequest).Name);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
```

---

# 7. TESTES

## 7.1 Unit Tests - Domain

```csharp
// tests/SGC.Clinica.Domain.Tests/Models/PatientTests.cs
public class PatientTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var name = "João Silva";
        var dateOfBirth = new DateTime(1990, 5, 15);
        var phone = "(11) 98765-4321";
        var document = "12345678901";

        // Act
        var result = Patient.Create(name, dateOfBirth, phone, document);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(name, result.Value.Name);
    }

    [Fact]
    public void Create_WithEmptyName_ReturnsFail sure()
    {
        // Arrange
        var name = "";
        var dateOfBirth = new DateTime(1990, 5, 15);
        var phone = "(11) 98765-4321";
        var document = "12345678901";

        // Act
        var result = Patient.Create(name, dateOfBirth, phone, document);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors);
        Assert.Any(result.Errors, e => e.Code == "NameRequired");
    }

    [Theory]
    [InlineData("CPF")]
    [InlineData("12")]
    [InlineData("12345678901234567890")]
    public void Create_WithInvalidDocument_ReturnsFail(string document)
    {
        // Arrange
        var name = "João Silva";
        var dateOfBirth = new DateTime(1990, 5, 15);
        var phone = "(11) 98765-4321";

        // Act
        var result = Patient.Create(name, dateOfBirth, phone, document);

        // Assert
        Assert.False(result.IsSuccess);
    }
}

// tests/SGC.Clinica.Domain.Tests/Models/AppointmentTests.cs
public class AppointmentTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var patientId = 1;
        var professionalId = 1;
        var scheduledDate = DateTime.UtcNow.AddDays(7);
        var duration = TimeSpan.FromMinutes(60);

        // Act
        var result = Appointment.Create(patientId, professionalId, scheduledDate, duration);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Pending, result.Value.Status);
    }

    [Fact]
    public void Confirm_FromPendingStatus_ReturnsSuccess()
    {
        // Arrange
        var appointmentResult = Appointment.Create(1, 1, DateTime.UtcNow.AddDays(7), TimeSpan.FromHours(1));
        var appointment = appointmentResult.Value;

        // Act
        var confirmResult = appointment.Confirm();

        // Assert
        Assert.True(confirmResult.IsSuccess);
        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void HasConflictWith_WithOverlappingAppointments_ReturnsTrue()
    {
        // Arrange
        var baseDate = DateTime.UtcNow.AddDays(1);
        var appointment1Result = Appointment.Create(1, 1, baseDate, TimeSpan.FromHours(1));
        var appointment2Result = Appointment.Create(2, 1, baseDate.AddMinutes(30), TimeSpan.FromHours(1));

        // Act
        var hasConflict = appointment1Result.Value.HasConflictWith(appointment2Result.Value);

        // Assert
        Assert.True(hasConflict);
    }
}
```

## 7.2 Integration Tests - Application

```csharp
// tests/SGC.Clinica.API.Tests/Integration/Patients/CreatePatientTests.cs
public class CreatePatientTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private IServiceScope _scope;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _scope?.Dispose();
        _factory?.Dispose();
    }

    [Fact]
    public async Task CreatePatient_WithValidData_ReturnsCreated()
    {
        // Arrange
        var command = new CreatePatientDto
        {
            Name = "João Silva",
            DateOfBirth = new DateTime(1990, 5, 15),
            Phone = "(11) 98765-4321",
            Document = "12345678901"
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/patients", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task CreatePatient_WithInvalidDocument_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreatePatientDto
        {
            Name = "João Silva",
            DateOfBirth = new DateTime(1990, 5, 15),
            Phone = "(11) 98765-4321",
            Document = "00000000000" // Invalid CPF
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/patients", content);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

---

# 8. CHECKLIST FINAL

## 8.1 Fase 0 - Refatoração Arquitetural

- [x ] Criar 4 projetos (Domain, Application, Infrastructure, API)
- [ ] Criar 3 projetos de teste (Domain.Tests, Application.Tests, API.Tests)
- [ ] Instalar todas as dependências NuGet
- [ ] Implementar Result Pattern em Domain
- [ ] Implementar Guard Clauses com Result<T>
- [ ] Implementar Value Objects (Email, CPF, PhoneNumber)
- [ ] Implementar Domain Events base (IDomainEvent)
- [ ] Implementar EF Core Configurations (5 modelos)
- [ ] Implementar UnitOfWork Pattern
- [ ] Implementar Repository Pattern (Generic + Specific)
- [ ] Executar migrations: `dotnet ef migrations add InitialCreate`
- [ ] Testar build: `dotnet build`

## 8.2 Fase 1 - Application & Handlers

- [ ] Implementar MediatR Pipeline
- [ ] Criar Behaviors (Validation, Logging, Transaction)
- [ ] Implementar Command Validators (Fluent)
- [ ] Implementar Query/Command Handlers
- [ ] Criar DTOs com AutoMapper profiles
- [ ] Implementar Event Handlers (Domain → RabbitMQ)

## 8.3 Fase 2.1 - Schedules & Appointments

- [ ] Implementar Schedule model com guard clauses
- [ ] Implementar Appointment model com guard clauses
- [ ] Criar Schedule Commands/Queries/Handlers
- [ ] Criar Appointment Commands/Queries/Handlers
- [ ] Implementar conflict detection
- [ ] Criar Schedule/Appointment Validators
- [ ] Implementar Schedule/Appointment Events
- [ ] Atualizar controllers com novos endpoints

## 8.4 Fase 3 - Infraestrutura Avançada

- [ ] Configurar RabbitMQ (docker-compose)
- [ ] Implementar RabbitMQ Publisher
- [ ] Implementar RabbitMQ Consumers
- [ ] Configurar Serilog estruturado
- [ ] Implementar JWT Authentication
- [ ] Implementar RBAC (Roles & Permissions)
- [ ] Implementar Health Checks (SQL, RabbitMQ)
- [ ] Implementar Correlation IDs para tracing

## 8.5 Fase 4 - Testes

- [ ] Escrever 20+ unit tests (Domain)
- [ ] Escrever 15+ unit tests (Application)
- [ ] Escrever 10+ integration tests (API)
- [ ] Atingir 80%+ code coverage
- [ ] Configurar CI/CD (GitHub Actions)

## 8.6 Fase 5 - Produção

- [ ] Documentação completa (README, API docs)
- [ ] Docker setup (Dockerfile, docker-compose)
- [ ] Environment configurations (.env files)
- [ ] Seeding scripts (dados iniciais)
- [ ] Backup/Recovery procedures

---

## 🎯 Próximos Passos

1. **Confirme** se essa arquitetura alinha com sua visão
2. **Comece com Fase 0** - Criar projetos e estrutura base
3. **Implemente Result Pattern** - Core para guard clauses
4. **Migre modelos existentes** para novo Project
5. **Teste cada fase** antes de prosseguir

---

**Documento:** Guia Arquitetural Sênior - SGC.Clínica  
**Data:** Janeiro 2026  
**Status:** Pronto para Implementação
