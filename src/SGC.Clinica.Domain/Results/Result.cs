namespace SGC.Clinica.Domain.Results
{
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<IError> Errors { get; }

        private Result(bool isSuccess, IReadOnlyList<IError> errors)
        {
            if (isSuccess && errors.Count > 0)
                throw new InvalidOperationException("Success result cannot contain errors.");

            if (!isSuccess && errors.Count == 0)
                throw new InvalidOperationException("Failure result must contain at least one error.");

            IsSuccess = isSuccess;
            Errors = errors;
        }

        public static Result Success()
            => new(true, Array.Empty<IError>());

        public static Result Failure(params IError[] errors)
            => new(false, errors);
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public IReadOnlyList<IError> Errors { get; }

        private Result(bool isSuccess, T value, IReadOnlyList<IError> errors)
        {
            if (isSuccess && errors.Count > 0)
                throw new InvalidOperationException("Success result cannot contain errors.");

            if (!isSuccess && errors.Count == 0)
                throw new InvalidOperationException("Failure result must contain at least one error.");

            IsSuccess = isSuccess;
            Value = value;
            Errors = errors;
        }

        public static Result<T> Success(T value)
            => new(true, value, Array.Empty<IError>());

        public static Result<T> Failure(params IError[] errors)
            => new(false, default!, errors);
    }
}

