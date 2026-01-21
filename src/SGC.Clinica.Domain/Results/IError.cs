using SGC.Clinica.Domain.Enums;

namespace SGC.Clinica.Domain.Results
{
    public interface IError
    {
        string Code { get; }
        string Message { get; }
        ErrorType Type { get; }
    }
}