using System;

namespace SAUEP.Core.Services
{
    public class Result
    {

        public bool IsSuccessful { get; }
        public Exception Exception { get; }

        public static Result Successful() => new Result(true);
        public static Result Failed(Exception exception) => new Result(false, exception);

        protected Result(bool isSuccessful, Exception exception = null)
        {
            IsSuccessful = isSuccessful;

            Exception = exception;
        }

        public static bool operator !(Result result) => !result.IsSuccessful;
        public static bool operator true(Result result) => result.IsSuccessful;
        public static bool operator false(Result result) => !result.IsSuccessful;
    }

    public sealed class Result<T> : Result
    {
        public T Value { get; }

        protected Result(bool isSuccessful, Exception exception, T result) : base(isSuccessful, exception)
        {
            Value = result;
        }

        public static new Result Successful() => Successful(default(T));
        public static Result<T> Successful(T value) => new Result<T>(true, null, value);

        public static new Result<T> Failed(Exception exception) => new Result<T>(false, exception, default(T));
    }
}
