using System;
using System.Threading.Tasks;

namespace Rustify.Monads
{
    public static class OptionExtensions
    {
        public static Option<TResult> Select<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, TResult> selector)
            where TSource : notnull
            where TResult : notnull
        {
            return source.Map(selector);
        }

        public static Option<TResult> SelectMany<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, Option<TResult>> selector)
            where TSource : notnull
            where TResult : notnull
        {
            return source.AndThen(selector);
        }

        public static Option<TResult> SelectMany<TSource, TCollection, TResult>(
            this Option<TSource> source,
            Func<TSource, Option<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
            where TSource : notnull
            where TCollection : notnull
            where TResult : notnull
        {
            return source.AndThen(s =>
                collectionSelector(s).Map(c => resultSelector(s, c)));
        }

        public static Option<T> Where<T>(
            this Option<T> source,
            Func<T, bool> predicate)
            where T : notnull
        {
            return source.Filter(predicate);
        }

        public static async Task<Option<TResult>> MapAsync<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, Task<TResult>> selector)
            where TSource : notnull
            where TResult : notnull
        {
            if (source.IsSome())
            {
                var result = await selector(source.Unwrap()).ConfigureAwait(false);
                return Option<TResult>.Some(result);
            }
            return Option<TResult>.None;
        }

        public static async Task<Option<TResult>> AndThenAsync<TSource, TResult>(
            this Option<TSource> source,
            Func<TSource, Task<Option<TResult>>> selector)
            where TSource : notnull
            where TResult : notnull
        {
            if (source.IsSome())
            {
                return await selector(source.Unwrap()).ConfigureAwait(false);
            }
            return Option<TResult>.None;
        }

        public static async Task<Option<TResult>> MapAsync<TSource, TResult>(
            this Task<Option<TSource>> sourceTask,
            Func<TSource, TResult> selector)
            where TSource : notnull
            where TResult : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            return source.Map(selector);
        }

        public static async Task<Option<TResult>> AndThenAsync<TSource, TResult>(
            this Task<Option<TSource>> sourceTask,
            Func<TSource, Option<TResult>> selector)
            where TSource : notnull
            where TResult : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            return source.AndThen(selector);
        }

        public static async Task<T> UnwrapOrAsync<T>(
            this Task<Option<T>> sourceTask,
            T defaultValue)
            where T : notnull
        {
            var source = await sourceTask.ConfigureAwait(false);
            return source.UnwrapOr(defaultValue);
        }
    }
}
