using System;
using System.Collections;
using System.Collections.Generic;

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

    public readonly struct Result<T, E> : IEnumerable<T>, IEquatable<Result<T, E>>, IComparable<Result<T, E>>
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

        public T Unwrap()
        {
            if (this.tag == 0)
            {
                throw new InvalidOperationException("Cannot unwrap an Error result");
            }

            return this.value;
        }

        public T Expect(string message)
        {
            if (this.tag == 0)
            {
                throw new InvalidOperationException(message);
            }

            return this.value;
        }

        public T UnwrapOr(T fallback)
        {
            if (this.tag == 0)
            {
                return fallback;
            }

            return this.value;
        }

        public T UnwrapOrElse(Func<E, T> fallback)
        {
            if (this.tag == 0)
            {
                return fallback(this.error);
            }

            return this.value;
        }

        public T? UnwrapOrDefault()
        {
            if (this.tag == 0)
            {
                return default;
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

        public E ExpectErr(string message)
        {
            if (this.tag != 0)
            {
                throw new InvalidOperationException(message);
            }

            return this.error;
        }

        public bool IsOkAnd(Func<T, bool> f)
        {
            if (this.tag != 0)
            {
                return f(this.value);
            }

            return false;
        }

        public bool IsErrAnd(Func<E, bool> f)
        {
            if (this.tag == 0)
            {
                return f(this.error);
            }

            return false;
        }

        public Result<T, E> AndThen(Func<T, Result<T, E>> f)
        {
            if (this.tag != 0)
            {
                return f(this.value);
            }

            return this;
        }

        public Result<U, E> And<U>(Result<U, E> other) where U : notnull
        {
            if (this.tag != 0)
            {
                return other;
            }

            return Result<U, E>.Err(this.error);
        }

        public Result<T, E> Or(Result<T, E> other)
        {
            if (this.tag != 0)
            {
                return this;
            }

            return other;
        }

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

        public Result<T, E> Inspect(Action<T> action)
        {
            if (this.tag != 0)
            {
                action(this.value);
            }
            return this;
        }

        public Result<T, E> InspectErr(Action<E> action)
        {
            if (this.tag == 0)
            {
                action(this.error);
            }
            return this;
        }

        public bool Contains(T value)
        {
            if (this.tag == 0) return false;
            return EqualityComparer<T>.Default.Equals(this.value, value);
        }

        public bool ContainsErr(E error)
        {
            if (this.tag != 0) return false;
            return EqualityComparer<E>.Default.Equals(this.error, error);
        }

        public static Result<U, E> Flatten<U>(Result<Result<U, E>, E> result)
            where U : notnull
        {
            if (result.IsOk)
            {
                return result.Unwrap();
            }
            return Result<U, E>.Err(result.UnwrapErr());
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

        public IEnumerator<T> GetEnumerator()
        {
            if (this.tag != 0)
            {
                yield return this.value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(Result<T, E> left, Result<T, E> right)
        {
            if (left.tag != right.tag) return false;
            if (left.tag == 0) return left.error.Equals(right.error);
            return left.value.Equals(right.value);
        }

        public static bool operator !=(Result<T, E> left, Result<T, E> right)
        {
            return !(left == right);
        }

        public bool Equals(Result<T, E> other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return obj is Result<T, E> result && this == result;
        }

        public int CompareTo(Result<T, E> other)
        {
            if (this.tag == 0 && other.tag == 0)
            {
                if (this.error is IComparable<E> comparableErr)
                {
                    return comparableErr.CompareTo(other.error);
                }
                return Comparer<E>.Default.Compare(this.error, other.error);
            }

            if (this.tag == 0) return -1;
            if (other.tag == 0) return 1;

            if (this.value is IComparable<T> comparable)
            {
                return comparable.CompareTo(other.value);
            }

            return Comparer<T>.Default.Compare(this.value, other.value);
        }

        public override int GetHashCode()
        {
            return this.tag != 0
                ? HashCode.Combine(this.tag, this.value)
                : HashCode.Combine(this.tag, this.error);
        }

        public override string ToString()
        {
            return this.tag != 0 ? $"Ok({this.value})" : $"Err({this.error})";
        }
    }
}