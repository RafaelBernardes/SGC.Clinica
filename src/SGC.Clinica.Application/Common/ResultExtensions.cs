using SGC.Clinica.Domain.Results;

namespace SGC.Clinica.Application.Common.Results;

public static class ResultExtensions
{
    public static TResult Match<T, TResult>(
        this Result<T> result,
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<IError>, TResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Errors);
    }

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> next)
    {
        return result.IsFailure
            ? Result<TOut>.Failure(result.Errors.ToArray())
            : next(result.Value);
    }
}