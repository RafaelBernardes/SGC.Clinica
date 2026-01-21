using SGC.Clinica.Domain.Results;

namespace SGC.Clinica.Api.Domain.Helpers
{
    public static class ValidationHelper
    {
        public static Result Validate(params Result[] validations)
        {
            var errors = validations
                .Where(v => v.IsFailure)
                .SelectMany(v => v.Errors)
                .ToArray();

            return errors.Any()
                ? Result.Failure(errors)
                : Result.Success();
        }
    }
}