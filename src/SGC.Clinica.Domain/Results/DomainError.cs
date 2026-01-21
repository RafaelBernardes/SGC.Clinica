using SGC.Clinica.Domain.Enums;

namespace SGC.Clinica.Domain.Results
{
    public sealed record DomainError(string Code, string Message) : IError
    {
        public ErrorType Type => ErrorType.Conflict;

        public DomainError(string message) : this(nameof(DomainError), message) { }
    }
}