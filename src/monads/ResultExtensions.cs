using System;
using System.Threading.Tasks;

namespace Rustify.Monads
{
    public static class ResultExtensions
    {
        public static Result<TResult, TError> Select<TSource, TResult, TError>(
            this Result<TSource, TError> source,
            Func<TSource, TResult> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            return source.Map(selector);
        }

        public static Result<TResult, TError> SelectMany<TSource, TResult, TError>(
            this Result<TSource, TError> source,
            Func<TSource, Result<TResult, TError>> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            if (source.IsOk)
            {
                return selector(source.Unwrap());
            }
            return Result<TResult, TError>.Err(source.UnwrapErr());
        }

        public static Result<TResult, TError> SelectMany<TSource, TCollection, TResult, TError>(
            this Result<TSource, TError> source,
            Func<TSource, Result<TCollection, TError>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
            where TSource : notnull
            where TCollection : notnull
            where TResult : notnull
            where TError : notnull
        {
            if (source.IsOk)
            {
                var sourceValue = source.Unwrap();
                var collectionResult = collectionSelector(sourceValue);
                if (collectionResult.IsOk)
                {
                    return Result<TResult, TError>.Ok(
                        resultSelector(sourceValue, collectionResult.Unwrap()));
                }
                return Result<TResult, TError>.Err(collectionResult.UnwrapErr());
            }
            return Result<TResult, TError>.Err(source.UnwrapErr());
        }

        public static async Task<Result<TResult, TError>> MapAsync<TSource, TResult, TError>(
            this Result<TSource, TError> source,
            Func<TSource, Task<TResult>> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            if (source.IsOk)
            {
                var result = await selector(source.Unwrap()).ConfigureAwait(false);
                return Result<TResult, TError>.Ok(result);
            }
            return Result<TResult, TError>.Err(source.UnwrapErr());
        }

        public static async Task<Result<TResult, TError>> AndThenAsync<TSource, TResult, TError>(
            this Result<TSource, TError> source,
            Func<TSource, Task<Result<TResult, TError>>> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            if (source.IsOk)
            {
                return await selector(source.Unwrap()).ConfigureAwait(false);
            }
            return Result<TResult, TError>.Err(source.UnwrapErr());
        }

        public static async Task<Result<TResult, TError>> MapAsync<TSource, TResult, TError>(
            this Task<Result<TSource, TError>> sourceTask,
            Func<TSource, TResult> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            return source.Map(selector);
        }

        public static async Task<Result<TResult, TError>> AndThenAsync<TSource, TResult, TError>(
            this Task<Result<TSource, TError>> sourceTask,
            Func<TSource, Result<TResult, TError>> selector)
            where TSource : notnull
            where TResult : notnull
            where TError : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            if (source.IsOk)
            {
                return selector(source.Unwrap());
            }
            return Result<TResult, TError>.Err(source.UnwrapErr());
        }

        public static async Task<T> UnwrapOrAsync<T, E>(
            this Task<Result<T, E>> sourceTask,
            T defaultValue)
            where T : notnull
            where E : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            return source.UnwrapOr(defaultValue);
        }
    }
}
