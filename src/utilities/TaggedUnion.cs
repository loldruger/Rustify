using System;
using System.Collections.Generic;

namespace Rustify.Utilities
{
    /// <summary>
    /// Static factory methods for creating TaggedUnion instances.
    /// </summary>
    public static class TaggedUnion
    {
        /// <summary>
        /// Creates a TaggedUnion containing a value of type A.
        /// </summary>
        public static TaggedUnion<A> _0<A>(A a)
            where A : notnull
        {
            return TaggedUnion<A>.__0(a);
        }

        /// <summary>
        /// Creates a 2-ary TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B> _0<A, B>(A a)
            where A : notnull
            where B : notnull
        {
            return TaggedUnion<A, B>.__0(a);
        }

        /// <summary>
        /// Creates a 2-ary TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B> _1<A, B>(B b)
            where A : notnull
            where B : notnull
        {
            return TaggedUnion<A, B>.__1(b);
        }

        /// <summary>
        /// Creates a 3-ary TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B, C> _0<A, B, C>(A a)
            where A : notnull
            where B : notnull
            where C : notnull
        {
            return TaggedUnion<A, B, C>.__0(a);
        }

        /// <summary>
        /// Creates a 3-ary TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B, C> _1<A, B, C>(B b)
            where A : notnull
            where B : notnull
            where C : notnull
        {
            return TaggedUnion<A, B, C>.__1(b);
        }

        /// <summary>
        /// Creates a 3-ary TaggedUnion containing a value of type C (third case).
        /// </summary>
        public static TaggedUnion<A, B, C> _2<A, B, C>(C c)
            where A : notnull
            where B : notnull
            where C : notnull
        {
            return TaggedUnion<A, B, C>.__2(c);
        }

        /// <summary>
        /// Creates a 4-ary TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B, C, D> _0<A, B, C, D>(A a)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
        {
            return TaggedUnion<A, B, C, D>.__0(a);
        }

        /// <summary>
        /// Creates a 4-ary TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B, C, D> _1<A, B, C, D>(B b)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
        {
            return TaggedUnion<A, B, C, D>.__1(b);
        }

        /// <summary>
        /// Creates a 4-ary TaggedUnion containing a value of type C (third case).
        /// </summary>
        public static TaggedUnion<A, B, C, D> _2<A, B, C, D>(C c)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
        {
            return TaggedUnion<A, B, C, D>.__2(c);
        }

        /// <summary>
        /// Creates a 4-ary TaggedUnion containing a value of type D (fourth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D> _3<A, B, C, D>(D d)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
        {
            return TaggedUnion<A, B, C, D>.__3(d);
        }

        /// <summary>
        /// Creates a 5-ary TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E> _0<A, B, C, D, E>(A a)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
        {
            return TaggedUnion<A, B, C, D, E>.__0(a);
        }

        /// <summary>
        /// Creates a 5-ary TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E> _1<A, B, C, D, E>(B b)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
        {
            return TaggedUnion<A, B, C, D, E>.__1(b);
        }

        /// <summary>
        /// Creates a 5-ary TaggedUnion containing a value of type C (third case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E> _2<A, B, C, D, E>(C c)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
        {
            return TaggedUnion<A, B, C, D, E>.__2(c);
        }

        /// <summary>
        /// Creates a 5-ary TaggedUnion containing a value of type D (fourth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E> _3<A, B, C, D, E>(D d)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
        {
            return TaggedUnion<A, B, C, D, E>.__3(d);
        }

        /// <summary>
        /// Creates a 5-ary TaggedUnion containing a value of type E (fifth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E> _4<A, B, C, D, E>(E e)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
        {
            return TaggedUnion<A, B, C, D, E>.__4(e);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _0<A, B, C, D, E, F>(A a)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__0(a);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _1<A, B, C, D, E, F>(B b)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__1(b);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type C (third case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _2<A, B, C, D, E, F>(C c)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__2(c);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type D (fourth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _3<A, B, C, D, E, F>(D d)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__3(d);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type E (fifth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _4<A, B, C, D, E, F>(E e)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__4(e);
        }

        /// <summary>
        /// Creates a 6-ary TaggedUnion containing a value of type F (sixth case).
        /// </summary>
        public static TaggedUnion<A, B, C, D, E, F> _5<A, B, C, D, E, F>(F f)
            where A : notnull
            where B : notnull
            where C : notnull
            where D : notnull
            where E : notnull
            where F : notnull
        {
            return TaggedUnion<A, B, C, D, E, F>.__5(f);
        }
    }

    /// <summary>
    /// A 1-ary tagged union (discriminated union) that can hold a value of type A.
    /// </summary>
    /// <typeparam name="A">The type of the value.</typeparam>
    public readonly struct TaggedUnion<A> : IEquatable<TaggedUnion<A>>
        where A : notnull
    {
        private readonly byte tag;
        private readonly A _0;

        private TaggedUnion(byte tag, A _0)
        {
            this.tag = tag;
            this._0 = _0;
        }

        /// <summary>
        /// Creates a TaggedUnion containing the specified value.
        /// </summary>
        public static TaggedUnion<A> __0(A b) => new(0, b);

        /// <summary>
        /// Implicitly converts a value to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A>(A a) => __0(a);

        /// <summary>
        /// Pattern matches on the union and returns the result of applying the matching function.
        /// </summary>
        public R Match<R>(Func<A, R> _0)
        {
            return this.tag switch
            {
                0 => _0(this._0),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        public bool Equals(TaggedUnion<A> other) =>
            this.tag == other.tag && EqualityComparer<A>.Default.Equals(this._0, other._0);

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A> u && this.Equals(u);

        public override int GetHashCode() => HashCode.Combine(this.tag, this._0);

        public override string ToString() => $"Case0({this._0})";

        public static bool operator ==(TaggedUnion<A> left, TaggedUnion<A> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A> left, TaggedUnion<A> right) => !left.Equals(right);
    }

    /// <summary>
    /// A 2-ary tagged union (discriminated union) that can hold either a value of type A or type B.
    /// Similar to Rust's enum with two variants.
    /// </summary>
    /// <typeparam name="A">The type of the first case.</typeparam>
    /// <typeparam name="B">The type of the second case.</typeparam>
    public readonly struct TaggedUnion<A, B> : IEquatable<TaggedUnion<A, B>>
        where A : notnull
        where B : notnull
    {
        private readonly byte tag;
        private readonly A _0;
        private readonly B _1;

        private TaggedUnion(byte tag, A _0, B _1)
        {
            this.tag = tag;
            this._0 = _0;
            this._1 = _1;
        }

        /// <summary>
        /// Creates a TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B> __0(A a) => new(0, a, default!);

        /// <summary>
        /// Creates a TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B> __1(B b) => new(1, default!, b);

        /// <summary>
        /// Implicitly converts a value of type A to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A, B>(A a) => __0(a);

        /// <summary>
        /// Implicitly converts a value of type B to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A, B>(B b) => __1(b);

        /// <summary>
        /// Returns true if this union contains a value of type A (first case).
        /// </summary>
        public bool Is0 => this.tag == 0;

        /// <summary>
        /// Returns true if this union contains a value of type B (second case).
        /// </summary>
        public bool Is1 => this.tag == 1;

        /// <summary>
        /// Pattern matches on the union and returns the result of applying the matching function.
        /// </summary>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="case0">Function to apply if the union contains a value of type A.</param>
        /// <param name="case1">Function to apply if the union contains a value of type B.</param>
        /// <returns>The result of the matching function.</returns>
        public R Match<R>(Func<A, R> case0, Func<B, R> case1)
        {
            return this.tag switch
            {
                0 => case0(this._0),
                1 => case1(this._1),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        /// <summary>
        /// Pattern matches on the union and executes the matching action.
        /// </summary>
        /// <param name="case0">Action to execute if the union contains a value of type A.</param>
        /// <param name="case1">Action to execute if the union contains a value of type B.</param>
        public void Match(Action<A> case0, Action<B> case1)
        {
            switch (this.tag)
            {
                case 0:
                    case0(this._0);
                    break;
                case 1:
                    case1(this._1);
                    break;
                default:
                    throw new InvalidOperationException("Invalid tag value");
            }
        }

        public bool Equals(TaggedUnion<A, B> other) =>
            this.tag == other.tag && this.tag switch
            {
                0 => EqualityComparer<A>.Default.Equals(this._0, other._0),
                1 => EqualityComparer<B>.Default.Equals(this._1, other._1),
                _ => false
            };

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A, B> u && this.Equals(u);

        public override int GetHashCode() => this.tag switch
        {
            0 => HashCode.Combine(0, this._0),
            1 => HashCode.Combine(1, this._1),
            _ => 0
        };

        public override string ToString() => this.tag switch
        {
            0 => $"Case0({this._0})",
            1 => $"Case1({this._1})",
            _ => "Invalid"
        };

        public static bool operator ==(TaggedUnion<A, B> left, TaggedUnion<A, B> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A, B> left, TaggedUnion<A, B> right) => !left.Equals(right);
    }

    /// <summary>
    /// A 3-ary tagged union (discriminated union) that can hold a value of type A, B, or C.
    /// Similar to Rust's enum with three variants.
    /// </summary>
    /// <typeparam name="A">The type of the first case.</typeparam>
    /// <typeparam name="B">The type of the second case.</typeparam>
    /// <typeparam name="C">The type of the third case.</typeparam>
    public readonly struct TaggedUnion<A, B, C> : IEquatable<TaggedUnion<A, B, C>>
        where A : notnull
        where B : notnull
        where C : notnull
    {
        private readonly byte tag;
        private readonly A _0;
        private readonly B _1;
        private readonly C _2;

        private TaggedUnion(byte tag, A _0, B _1, C _2)
        {
            this.tag = tag;
            this._0 = _0;
            this._1 = _1;
            this._2 = _2;
        }

        /// <summary>
        /// Creates a TaggedUnion containing a value of type A (first case).
        /// </summary>
        public static TaggedUnion<A, B, C> __0(A a) => new(0, a, default!, default!);

        /// <summary>
        /// Creates a TaggedUnion containing a value of type B (second case).
        /// </summary>
        public static TaggedUnion<A, B, C> __1(B b) => new(1, default!, b, default!);

        /// <summary>
        /// Creates a TaggedUnion containing a value of type C (third case).
        /// </summary>
        public static TaggedUnion<A, B, C> __2(C c) => new(2, default!, default!, c);

        /// <summary>
        /// Implicitly converts a value of type A to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A, B, C>(A a) => __0(a);

        /// <summary>
        /// Implicitly converts a value of type B to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A, B, C>(B b) => __1(b);

        /// <summary>
        /// Implicitly converts a value of type C to a TaggedUnion.
        /// </summary>
        public static implicit operator TaggedUnion<A, B, C>(C c) => __2(c);

        /// <summary>
        /// Returns true if this union contains a value of type A (first case).
        /// </summary>
        public bool Is0 => this.tag == 0;

        /// <summary>
        /// Returns true if this union contains a value of type B (second case).
        /// </summary>
        public bool Is1 => this.tag == 1;

        /// <summary>
        /// Returns true if this union contains a value of type C (third case).
        /// </summary>
        public bool Is2 => this.tag == 2;

        /// <summary>
        /// Pattern matches on the union and returns the result of applying the matching function.
        /// </summary>
        /// <typeparam name="R">The return type.</typeparam>
        /// <param name="case0">Function to apply if the union contains a value of type A.</param>
        /// <param name="case1">Function to apply if the union contains a value of type B.</param>
        /// <param name="case2">Function to apply if the union contains a value of type C.</param>
        /// <returns>The result of the matching function.</returns>
        public R Match<R>(Func<A, R> case0, Func<B, R> case1, Func<C, R> case2)
        {
            return this.tag switch
            {
                0 => case0(this._0),
                1 => case1(this._1),
                2 => case2(this._2),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        /// <summary>
        /// Pattern matches on the union and executes the matching action.
        /// </summary>
        /// <param name="case0">Action to execute if the union contains a value of type A.</param>
        /// <param name="case1">Action to execute if the union contains a value of type B.</param>
        /// <param name="case2">Action to execute if the union contains a value of type C.</param>
        public void Match(Action<A> case0, Action<B> case1, Action<C> case2)
        {
            switch (this.tag)
            {
                case 0:
                    case0(this._0);
                    break;
                case 1:
                    case1(this._1);
                    break;
                case 2:
                    case2(this._2);
                    break;
                default:
                    throw new InvalidOperationException("Invalid tag value");
            }
        }

        public bool Equals(TaggedUnion<A, B, C> other) =>
            this.tag == other.tag && this.tag switch
            {
                0 => EqualityComparer<A>.Default.Equals(this._0, other._0),
                1 => EqualityComparer<B>.Default.Equals(this._1, other._1),
                2 => EqualityComparer<C>.Default.Equals(this._2, other._2),
                _ => false
            };

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A, B, C> u && this.Equals(u);

        public override int GetHashCode() => this.tag switch
        {
            0 => HashCode.Combine(0, this._0),
            1 => HashCode.Combine(1, this._1),
            2 => HashCode.Combine(2, this._2),
            _ => 0
        };

        public override string ToString() => this.tag switch
        {
            0 => $"Case0({this._0})",
            1 => $"Case1({this._1})",
            2 => $"Case2({this._2})",
            _ => "Invalid"
        };

        public static bool operator ==(TaggedUnion<A, B, C> left, TaggedUnion<A, B, C> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A, B, C> left, TaggedUnion<A, B, C> right) => !left.Equals(right);
    }

    /// <summary>
    /// A 4-ary tagged union that can hold a value of type A, B, C, or D.
    /// </summary>
    public readonly struct TaggedUnion<A, B, C, D> : IEquatable<TaggedUnion<A, B, C, D>>
        where A : notnull
        where B : notnull
        where C : notnull
        where D : notnull
    {
        private readonly byte tag;
        private readonly A _0;
        private readonly B _1;
        private readonly C _2;
        private readonly D _3;

        private TaggedUnion(byte tag, A _0, B _1, C _2, D _3)
        {
            this.tag = tag;
            this._0 = _0;
            this._1 = _1;
            this._2 = _2;
            this._3 = _3;
        }

        public static TaggedUnion<A, B, C, D> __0(A a) => new(0, a, default!, default!, default!);
        public static TaggedUnion<A, B, C, D> __1(B b) => new(1, default!, b, default!, default!);
        public static TaggedUnion<A, B, C, D> __2(C c) => new(2, default!, default!, c, default!);
        public static TaggedUnion<A, B, C, D> __3(D d) => new(3, default!, default!, default!, d);

        public static implicit operator TaggedUnion<A, B, C, D>(A a) => __0(a);
        public static implicit operator TaggedUnion<A, B, C, D>(B b) => __1(b);
        public static implicit operator TaggedUnion<A, B, C, D>(C c) => __2(c);
        public static implicit operator TaggedUnion<A, B, C, D>(D d) => __3(d);

        public bool Is0 => this.tag == 0;
        public bool Is1 => this.tag == 1;
        public bool Is2 => this.tag == 2;
        public bool Is3 => this.tag == 3;

        public R Match<R>(Func<A, R> case0, Func<B, R> case1, Func<C, R> case2, Func<D, R> case3)
        {
            return this.tag switch
            {
                0 => case0(this._0),
                1 => case1(this._1),
                2 => case2(this._2),
                3 => case3(this._3),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        public void Match(Action<A> case0, Action<B> case1, Action<C> case2, Action<D> case3)
        {
            switch (this.tag)
            {
                case 0: case0(this._0); break;
                case 1: case1(this._1); break;
                case 2: case2(this._2); break;
                case 3: case3(this._3); break;
                default: throw new InvalidOperationException("Invalid tag value");
            }
        }

        public bool Equals(TaggedUnion<A, B, C, D> other) =>
            this.tag == other.tag && this.tag switch
            {
                0 => EqualityComparer<A>.Default.Equals(this._0, other._0),
                1 => EqualityComparer<B>.Default.Equals(this._1, other._1),
                2 => EqualityComparer<C>.Default.Equals(this._2, other._2),
                3 => EqualityComparer<D>.Default.Equals(this._3, other._3),
                _ => false
            };

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A, B, C, D> u && this.Equals(u);

        public override int GetHashCode() => this.tag switch
        {
            0 => HashCode.Combine(0, this._0),
            1 => HashCode.Combine(1, this._1),
            2 => HashCode.Combine(2, this._2),
            3 => HashCode.Combine(3, this._3),
            _ => 0
        };

        public override string ToString() => this.tag switch
        {
            0 => $"Case0({this._0})",
            1 => $"Case1({this._1})",
            2 => $"Case2({this._2})",
            3 => $"Case3({this._3})",
            _ => "Invalid"
        };

        public static bool operator ==(TaggedUnion<A, B, C, D> left, TaggedUnion<A, B, C, D> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A, B, C, D> left, TaggedUnion<A, B, C, D> right) => !left.Equals(right);
    }

    /// <summary>
    /// A 5-ary tagged union that can hold a value of type A, B, C, D, or E.
    /// </summary>
    public readonly struct TaggedUnion<A, B, C, D, E> : IEquatable<TaggedUnion<A, B, C, D, E>>
        where A : notnull
        where B : notnull
        where C : notnull
        where D : notnull
        where E : notnull
    {
        private readonly byte tag;
        private readonly A _0;
        private readonly B _1;
        private readonly C _2;
        private readonly D _3;
        private readonly E _4;

        private TaggedUnion(byte tag, A _0, B _1, C _2, D _3, E _4)
        {
            this.tag = tag;
            this._0 = _0;
            this._1 = _1;
            this._2 = _2;
            this._3 = _3;
            this._4 = _4;
        }

        public static TaggedUnion<A, B, C, D, E> __0(A a) => new(0, a, default!, default!, default!, default!);
        public static TaggedUnion<A, B, C, D, E> __1(B b) => new(1, default!, b, default!, default!, default!);
        public static TaggedUnion<A, B, C, D, E> __2(C c) => new(2, default!, default!, c, default!, default!);
        public static TaggedUnion<A, B, C, D, E> __3(D d) => new(3, default!, default!, default!, d, default!);
        public static TaggedUnion<A, B, C, D, E> __4(E e) => new(4, default!, default!, default!, default!, e);

        public static implicit operator TaggedUnion<A, B, C, D, E>(A a) => __0(a);
        public static implicit operator TaggedUnion<A, B, C, D, E>(B b) => __1(b);
        public static implicit operator TaggedUnion<A, B, C, D, E>(C c) => __2(c);
        public static implicit operator TaggedUnion<A, B, C, D, E>(D d) => __3(d);
        public static implicit operator TaggedUnion<A, B, C, D, E>(E e) => __4(e);

        public bool Is0 => this.tag == 0;
        public bool Is1 => this.tag == 1;
        public bool Is2 => this.tag == 2;
        public bool Is3 => this.tag == 3;
        public bool Is4 => this.tag == 4;

        public R Match<R>(Func<A, R> case0, Func<B, R> case1, Func<C, R> case2, Func<D, R> case3, Func<E, R> case4)
        {
            return this.tag switch
            {
                0 => case0(this._0),
                1 => case1(this._1),
                2 => case2(this._2),
                3 => case3(this._3),
                4 => case4(this._4),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        public void Match(Action<A> case0, Action<B> case1, Action<C> case2, Action<D> case3, Action<E> case4)
        {
            switch (this.tag)
            {
                case 0: case0(this._0); break;
                case 1: case1(this._1); break;
                case 2: case2(this._2); break;
                case 3: case3(this._3); break;
                case 4: case4(this._4); break;
                default: throw new InvalidOperationException("Invalid tag value");
            }
        }

        public bool Equals(TaggedUnion<A, B, C, D, E> other) =>
            this.tag == other.tag && this.tag switch
            {
                0 => EqualityComparer<A>.Default.Equals(this._0, other._0),
                1 => EqualityComparer<B>.Default.Equals(this._1, other._1),
                2 => EqualityComparer<C>.Default.Equals(this._2, other._2),
                3 => EqualityComparer<D>.Default.Equals(this._3, other._3),
                4 => EqualityComparer<E>.Default.Equals(this._4, other._4),
                _ => false
            };

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A, B, C, D, E> u && this.Equals(u);

        public override int GetHashCode() => this.tag switch
        {
            0 => HashCode.Combine(0, this._0),
            1 => HashCode.Combine(1, this._1),
            2 => HashCode.Combine(2, this._2),
            3 => HashCode.Combine(3, this._3),
            4 => HashCode.Combine(4, this._4),
            _ => 0
        };

        public override string ToString() => this.tag switch
        {
            0 => $"Case0({this._0})",
            1 => $"Case1({this._1})",
            2 => $"Case2({this._2})",
            3 => $"Case3({this._3})",
            4 => $"Case4({this._4})",
            _ => "Invalid"
        };

        public static bool operator ==(TaggedUnion<A, B, C, D, E> left, TaggedUnion<A, B, C, D, E> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A, B, C, D, E> left, TaggedUnion<A, B, C, D, E> right) => !left.Equals(right);
    }

    /// <summary>
    /// A 6-ary tagged union that can hold a value of type A, B, C, D, E, or F.
    /// </summary>
    public readonly struct TaggedUnion<A, B, C, D, E, F> : IEquatable<TaggedUnion<A, B, C, D, E, F>>
        where A : notnull
        where B : notnull
        where C : notnull
        where D : notnull
        where E : notnull
        where F : notnull
    {
        private readonly byte tag;
        private readonly A _0;
        private readonly B _1;
        private readonly C _2;
        private readonly D _3;
        private readonly E _4;
        private readonly F _5;

        private TaggedUnion(byte tag, A _0, B _1, C _2, D _3, E _4, F _5)
        {
            this.tag = tag;
            this._0 = _0;
            this._1 = _1;
            this._2 = _2;
            this._3 = _3;
            this._4 = _4;
            this._5 = _5;
        }

        public static TaggedUnion<A, B, C, D, E, F> __0(A a) => new(0, a, default!, default!, default!, default!, default!);
        public static TaggedUnion<A, B, C, D, E, F> __1(B b) => new(1, default!, b, default!, default!, default!, default!);
        public static TaggedUnion<A, B, C, D, E, F> __2(C c) => new(2, default!, default!, c, default!, default!, default!);
        public static TaggedUnion<A, B, C, D, E, F> __3(D d) => new(3, default!, default!, default!, d, default!, default!);
        public static TaggedUnion<A, B, C, D, E, F> __4(E e) => new(4, default!, default!, default!, default!, e, default!);
        public static TaggedUnion<A, B, C, D, E, F> __5(F f) => new(5, default!, default!, default!, default!, default!, f);

        public static implicit operator TaggedUnion<A, B, C, D, E, F>(A a) => __0(a);
        public static implicit operator TaggedUnion<A, B, C, D, E, F>(B b) => __1(b);
        public static implicit operator TaggedUnion<A, B, C, D, E, F>(C c) => __2(c);
        public static implicit operator TaggedUnion<A, B, C, D, E, F>(D d) => __3(d);
        public static implicit operator TaggedUnion<A, B, C, D, E, F>(E e) => __4(e);
        public static implicit operator TaggedUnion<A, B, C, D, E, F>(F f) => __5(f);

        public bool Is0 => this.tag == 0;
        public bool Is1 => this.tag == 1;
        public bool Is2 => this.tag == 2;
        public bool Is3 => this.tag == 3;
        public bool Is4 => this.tag == 4;
        public bool Is5 => this.tag == 5;

        public R Match<R>(Func<A, R> case0, Func<B, R> case1, Func<C, R> case2, Func<D, R> case3, Func<E, R> case4, Func<F, R> case5)
        {
            return this.tag switch
            {
                0 => case0(this._0),
                1 => case1(this._1),
                2 => case2(this._2),
                3 => case3(this._3),
                4 => case4(this._4),
                5 => case5(this._5),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }

        public void Match(Action<A> case0, Action<B> case1, Action<C> case2, Action<D> case3, Action<E> case4, Action<F> case5)
        {
            switch (this.tag)
            {
                case 0: case0(this._0); break;
                case 1: case1(this._1); break;
                case 2: case2(this._2); break;
                case 3: case3(this._3); break;
                case 4: case4(this._4); break;
                case 5: case5(this._5); break;
                default: throw new InvalidOperationException("Invalid tag value");
            }
        }

        public bool Equals(TaggedUnion<A, B, C, D, E, F> other) =>
            this.tag == other.tag && this.tag switch
            {
                0 => EqualityComparer<A>.Default.Equals(this._0, other._0),
                1 => EqualityComparer<B>.Default.Equals(this._1, other._1),
                2 => EqualityComparer<C>.Default.Equals(this._2, other._2),
                3 => EqualityComparer<D>.Default.Equals(this._3, other._3),
                4 => EqualityComparer<E>.Default.Equals(this._4, other._4),
                5 => EqualityComparer<F>.Default.Equals(this._5, other._5),
                _ => false
            };

        public override bool Equals(object? obj) =>
            obj is TaggedUnion<A, B, C, D, E, F> u && this.Equals(u);

        public override int GetHashCode() => this.tag switch
        {
            0 => HashCode.Combine(0, this._0),
            1 => HashCode.Combine(1, this._1),
            2 => HashCode.Combine(2, this._2),
            3 => HashCode.Combine(3, this._3),
            4 => HashCode.Combine(4, this._4),
            5 => HashCode.Combine(5, this._5),
            _ => 0
        };

        public override string ToString() => this.tag switch
        {
            0 => $"Case0({this._0})",
            1 => $"Case1({this._1})",
            2 => $"Case2({this._2})",
            3 => $"Case3({this._3})",
            4 => $"Case4({this._4})",
            5 => $"Case5({this._5})",
            _ => "Invalid"
        };

        public static bool operator ==(TaggedUnion<A, B, C, D, E, F> left, TaggedUnion<A, B, C, D, E, F> right) => left.Equals(right);
        public static bool operator !=(TaggedUnion<A, B, C, D, E, F> left, TaggedUnion<A, B, C, D, E, F> right) => !left.Equals(right);
    }
}
