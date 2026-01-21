using SGC.Clinica.Domain.Enums;

namespace SGC.Clinica.Domain.Results
{
    public sealed record ValidationError(string Code, string Message) : IError
    {
        public ErrorType Type => ErrorType.Validation;

        public ValidationError(string message) : this(nameof(ValidationError), message) { }
    }
}