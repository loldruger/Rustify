using System;

namespace Rustify.Monads
{
    public static class Option
    {
        public static Option<T> None<T>() where T : notnull
        {
            return Option<T>.None;
        }

        public static Option<T> Some<T>(T value) where T : notnull
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null");
            }
            return Option<T>.Some(value);
        }
    }

    public readonly struct Option<T> where T : notnull
    {
        private readonly byte tag;
        private readonly T value;

        public static Option<T> None => new(0, default!);
        public static Option<T> Some(T value) => new(1, value);

        private Option(byte tag, T value)
        {
            this.tag = tag;
            this.value = value;
        }

        public bool IsSome() => this.tag != 0;
        public bool IsNone() => this.tag == 0;

        public bool IsSomeAnd(Func<T, bool> f)
        {
            return this.Match(
                none: () => false,
                some: x => f(x)
            );
        }

        public bool IsNoneOr(Func<T, bool> f)
        {
            return this.Match(
                none: () => true,
                some: x => f(x)
            );
        }

        public T Unwrap()
        {
            if (this.tag == 0)
            {
                throw new InvalidOperationException("Option is none");
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

        public T UnwrapOrElse(Func<T> fallback)
        {
            if (this.tag == 0)
            {
                return fallback();
            }

            return this.value;
        }

        public Option<U> Map<U>(Func<T, U> f) where U : notnull
        {
            if (this.tag != 0)
            {
                return Option<U>.Some(f(this.value));
            }

            return Option<U>.None;
        }

        public Option<U> AndThen<U>(Func<T, Option<U>> f) where U : notnull
        {
            if (this.tag != 0)
            {
                return f(this.value);
            }

            return Option<U>.None;
        }

        public Option<T> OrElse(Func<Option<T>> fallback)
        {
            if (this.tag == 0)
            {
                return fallback();
            }

            return this;
        }

        /// <summary>
        /// Transforms the Option into a Result, Some(v) becomes Ok(v), None becomes Err(err) using the provided default err value.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Result<T, E> OkOr<E>(E error) where E : notnull
        {
            return this.tag switch
            {
                0 => Result<T, E>.Err(error),
                1 => Result<T, E>.Ok(this.value),
                _ => throw new InvalidOperationException("Invalid Option tag"),
            };
        }

        /// <summary>
        /// Transforms the Option into a Result, Some(v) becomes Ok(v), None becomes Err(err) using the provided function.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Result<T, E> OkOrElse<E>(Func<E> error) where E : notnull
        {
            return this.tag switch
            {
                0 => Result<T, E>.Err(error()),
                1 => Result<T, E>.Ok(this.value),
                _ => throw new InvalidOperationException("Invalid Option tag"),
            };
        }

        public R Match<R>(Func<T, R> some, Func<R> none)
        {
            return this.tag switch
            {
                0 => none(),
                1 => some(this.value),
                _ => throw new InvalidOperationException("Invalid Option tag"),
            };
        }

        public Option<T> Filter(Func<T, bool> predicate)
        {
            if (this.IsSomeAnd(predicate))
            {
                return this;
            }
            return Option<T>.None;
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.tag == right.tag && left.value.Equals(right.value);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> option && this == option;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.tag, this.value);
        }
    }
}