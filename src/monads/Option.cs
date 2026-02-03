using System;
using System.Collections;
using System.Collections.Generic;

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

    public readonly struct Option<T> : IEnumerable<T>, IEquatable<Option<T>>, IComparable<Option<T>> where T : notnull
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
             ArgumentNullException.ThrowIfNull(f);
             return this.Match(
                 none: () => false,
                 some: x => f(x)
             );
         }

         public bool IsNoneOr(Func<T, bool> f)
         {
             ArgumentNullException.ThrowIfNull(f);
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

         public T UnwrapOrElse(Func<T> fallback)
         {
             ArgumentNullException.ThrowIfNull(fallback);
             if (this.tag == 0)
             {
                 return fallback();
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

         public Option<U> Map<U>(Func<T, U> f) where U : notnull
         {
             ArgumentNullException.ThrowIfNull(f);
             if (this.tag != 0)
             {
                 return Option<U>.Some(f(this.value));
             }

             return Option<U>.None;
         }

         public U MapOr<U>(U defaultValue, Func<T, U> f)
         {
             ArgumentNullException.ThrowIfNull(f);
             if (this.tag != 0)
             {
                 return f(this.value);
             }

             return defaultValue;
         }

         public U MapOrElse<U>(Func<U> noneFn, Func<T, U> someFn)
         {
             ArgumentNullException.ThrowIfNull(noneFn);
             ArgumentNullException.ThrowIfNull(someFn);
             if (this.tag != 0)
             {
                 return someFn(this.value);
             }

             return noneFn();
         }

         public Option<U> AndThen<U>(Func<T, Option<U>> f) where U : notnull
         {
             ArgumentNullException.ThrowIfNull(f);
             if (this.tag != 0)
             {
                 return f(this.value);
             }

             return Option<U>.None;
         }

        public Option<T> Or(Option<T> other)
        {
            if (this.tag != 0)
            {
                return this;
            }

            return other;
        }

         public Option<T> OrElse(Func<Option<T>> f)
         {
             ArgumentNullException.ThrowIfNull(f);
             if (this.tag == 0)
             {
                 return f();
             }

             return this;
         }

        public Option<T> Xor(Option<T> other)
        {
            if (this.tag != 0 && other.tag == 0)
            {
                return this;
            }
            if (this.tag == 0 && other.tag != 0)
            {
                return other;
            }

            return Option<T>.None;
        }

        public Option<(T, U)> Zip<U>(Option<U> other) where U : notnull
        {
            if (this.tag != 0 && other.IsSome())
            {
                return Option<(T, U)>.Some((this.value, other.Unwrap()));
            }

            return Option<(T, U)>.None;
        }

         public Option<R> ZipWith<U, R>(Option<U> other, Func<T, U, R> f)
             where U : notnull
             where R : notnull
         {
             ArgumentNullException.ThrowIfNull(f);
             if (this.tag != 0 && other.IsSome())
             {
                 return Option<R>.Some(f(this.value, other.Unwrap()));
             }

             return Option<R>.None;
         }

        public Result<T, E> OkOr<E>(E error) where E : notnull
        {
            return this.tag switch
            {
                0 => Result<T, E>.Err(error),
                1 => Result<T, E>.Ok(this.value),
                _ => throw new InvalidOperationException("Invalid Option tag"),
            };
        }

         public Result<T, E> OkOrElse<E>(Func<E> err) where E : notnull
         {
             ArgumentNullException.ThrowIfNull(err);
             return this.tag switch
             {
                 0 => Result<T, E>.Err(err()),
                 1 => Result<T, E>.Ok(this.value),
                 _ => throw new InvalidOperationException("Invalid Option tag"),
             };
         }

         public R Match<R>(Func<T, R> some, Func<R> none)
         {
             ArgumentNullException.ThrowIfNull(none);
             ArgumentNullException.ThrowIfNull(some);
             return this.tag switch
             {
                 0 => none(),
                 1 => some(this.value),
                 _ => throw new InvalidOperationException("Invalid Option tag"),
             };
         }

         public Option<T> Filter(Func<T, bool> predicate)
         {
             ArgumentNullException.ThrowIfNull(predicate);
             if (this.IsSomeAnd(predicate))
             {
                 return this;
             }
             return Option<T>.None;
         }

        public bool Contains(T value)
        {
            if (this.tag == 0) return false;
            return EqualityComparer<T>.Default.Equals(this.value, value);
        }

         public Option<T> Inspect(Action<T> action)
         {
             ArgumentNullException.ThrowIfNull(action);
             if (this.tag != 0)
             {
                 action(this.value);
             }
             return this;
         }

        public static Option<T> Flatten(Option<Option<T>> option)
        {
            if (option.IsSome())
            {
                return option.Unwrap();
            }
            return Option<T>.None;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.tag != 0)
            {
                yield return this.value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            if (left.tag != right.tag) return false;
            if (left.tag == 0) return true;
            return left.value.Equals(right.value);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !(left == right);
        }

        public bool Equals(Option<T> other)
        {
            return this == other;
        }

        public override bool Equals(object? obj)
        {
            return obj is Option<T> option && this == option;
        }

        public int CompareTo(Option<T> other)
        {
            if (this.tag == 0 && other.tag == 0) return 0;
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
            return HashCode.Combine(this.tag, this.value);
        }

        public override string ToString()
        {
            return this.tag != 0 ? $"Some({this.value})" : "None";
        }
    }
}