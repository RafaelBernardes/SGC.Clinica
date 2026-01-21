using SGC.Clinica.Domain.Enums;

namespace SGC.Clinica.Domain.Results
{
    public sealed record NotFoundError(string Code, string Message) : IError
    {
        public ErrorType Type => ErrorType.NotFound;

        public NotFoundError(string message) : this(nameof(NotFoundError), message) { }
    }
}