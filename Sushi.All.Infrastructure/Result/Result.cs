using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sushi.All.Infrastructure.Result
{
    public readonly record struct Result
    {
        public bool IsSuccess { get; }
        public Error Error { get; }

        private Result(bool isSuccess, Error error)
            => (IsSuccess, Error) = (isSuccess, error);

        public static Result Ok() => new(true, Error.None);

        public static Result Fail(Error error)
            => new(false, error ?? throw new ArgumentNullException(nameof(error)));

        public static implicit operator Result(Error error) => Fail(error);
    }

    public readonly record struct Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error Error { get; }

        private Result(bool isSuccess, T? value, Error error)
            => (IsSuccess, Value, Error) = (isSuccess, value, error);

        public static Result<T> Ok(T value)
            => new(true, value, Error.None);

        public static Result<T> Fail(Error error)
            => new(false, default, error ?? throw new ArgumentNullException(nameof(error)));

        public Result<U> Map<U>(Func<T, U> mapper)
            => IsSuccess ? Result<U>.Ok(mapper(Value!)) : Result<U>.Fail(Error);

        public Result<U> Bind<U>(Func<T, Result<U>> binder)
            => IsSuccess ? binder(Value!) : Result<U>.Fail(Error);

        public TResult Match<TResult>(Func<T, TResult> onOk, Func<Error, TResult> onFail)
            => IsSuccess ? onOk(Value!) : onFail(Error);
    }
}
