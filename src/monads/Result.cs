using System;

namespace Rustify.Monads
{
    public static class Result
    {
        public static Result<T, E> Err<T, E>(E e)
            where T : notnull
            where E : notnull
        {
            return Result<T, E>.Err(e);
        }

        public static Result<T, E> Ok<T, E>(T v)
            where T : notnull
            where E : notnull
        {
            return Result<T, E>.Ok(v);
        }
    }

    public readonly struct Result<T, E>
        where T : notnull
        where E : notnull
    {
        private readonly byte tag;
        private readonly T value;
        private readonly E error;

        private Result(byte tag, T value, E error)
        {
            this.tag = tag;
            this.value = value;
            this.error = error;
        }

        public bool IsOk => this.tag != 0;
        public bool IsErr => this.tag == 0;

        public static Result<T, E> Ok(T v) => new(1, v, default!);
        public static Result<T, E> Err(E e) => new(0, default!, e);

        public static implicit operator Result<T, E>(T v) => Ok(v);
        public static implicit operator Result<T, E>(E e) => Err(e);

        public R Match<R>(
                Func<T, R> success,
                Func<E, R> failure
            )
        {
            if (this.tag != 0)
            {
                return success(this.value);
            }
            else
            {
                return failure(this.error);
            }
        }

        /// <throw>InvalidOperationException</throw>
        public T Unwrap()
        {
            if (this.tag == 0)
            {
                throw new InvalidOperationException("Cannot unwrap an Error result");
            }

            return this.value;
        }

        public E UnwrapErr()
        {
            if (this.tag != 0)
            {
                throw new InvalidOperationException("Cannot unwrap an Ok result as Error");
            }

            return this.error;
        }

        public Result<T, E> AndThen(Func<T, Result<T, E>> f)
        {
            if (this.tag != 0)
            {
                return f(this.value);
            }

            return this;
        }

        /// <summary>
        /// Calls `op` if the result is [`Err`], otherwise returns the [`Ok`] value of `self`.
        /// </summary>
        /// <typeparam name="F"></typeparam>
        /// <param name="op"></param>
        /// <returns></returns>
        public Result<T, F> OrElse<F>(Func<E, Result<T, F>> op)
            where F : notnull
        {
            if (this.tag == 0)
            {
                return op(this.error);
            }

            return Result<T, F>.Ok(this.value);
        }

        public Option<T> Ok()
        {
            if (this.tag != 0)
            {
                return Option.Some(this.value);
            }

            return Option.None<T>();
        }

        public Option<E> Err()
        {
            if (this.tag == 0)
            {
                return Option.Some(this.error);
            }

            return Option.None<E>();
        }

        public Result<U, E> Map<U>(Func<T, U> f) where U : notnull
        {
            if (this.tag != 0)
            {
                return Result<U, E>.Ok(f(this.value));
            }

            return Result<U, E>.Err(this.error);
        }

        public Result<T, F> MapErr<F>(Func<E, F> f) where F : notnull
        {
            if (this.tag == 0)
            {
                return Result<T, F>.Err(f(this.error));
            }

            return Result<T, F>.Ok(this.value);
        }

        public U MapOr<U>(U defaultValue, Func<T, U> mapFn) where U : notnull
        {
            if (this.IsOk)
            {
                return mapFn(this.value);
            }
            return defaultValue;
        }

        public U MapOrElse<U>(Func<E, U> errFn, Func<T, U> okFn) where U : notnull
        {
            if (this.IsOk)
            {
                return okFn(this.value);
            }
            return errFn(this.error);
        }

        public static Result<U, E> Flatten<U>(Result<Result<U, E>, E> result)
            where U : notnull
        {
            if (result.IsOk)
            {
                return result.Unwrap(); // Returns the inner Result<U, E>
            }
            return Result<U, E>.Err(result.UnwrapErr()); // Returns the outer Err
        }

        public static Option<Result<U, E>> Transpose<U>(Result<Option<U>, E> result)
            where U : notnull
        {
            if (result.IsOk)
            {
                var option = result.Unwrap();
                if (option.IsSome())
                {
                    return Option.Some(Result<U, E>.Ok(option.Unwrap()));
                }
                else
                {
                    return Option.None<Result<U, E>>();
                }
            }
            else
            {
                return Option.Some(Result<U, E>.Err(result.UnwrapErr()));
            }
        }
    }
}