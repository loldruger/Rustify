using System;

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
    }

    /// <summary>
    /// A 1-ary tagged union (discriminated union) that can hold a value of type A.
    /// </summary>
    /// <typeparam name="A">The type of the value.</typeparam>
    public readonly struct TaggedUnion<A>
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
    }

    /// <summary>
    /// A 2-ary tagged union (discriminated union) that can hold either a value of type A or type B.
    /// Similar to Rust's enum with two variants.
    /// </summary>
    /// <typeparam name="A">The type of the first case.</typeparam>
    /// <typeparam name="B">The type of the second case.</typeparam>
    public readonly struct TaggedUnion<A, B>
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
        public bool Is0 => tag == 0;

        /// <summary>
        /// Returns true if this union contains a value of type B (second case).
        /// </summary>
        public bool Is1 => tag == 1;

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
    }

    /// <summary>
    /// A 3-ary tagged union (discriminated union) that can hold a value of type A, B, or C.
    /// Similar to Rust's enum with three variants.
    /// </summary>
    /// <typeparam name="A">The type of the first case.</typeparam>
    /// <typeparam name="B">The type of the second case.</typeparam>
    /// <typeparam name="C">The type of the third case.</typeparam>
    public readonly struct TaggedUnion<A, B, C>
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
        public bool Is0 => tag == 0;

        /// <summary>
        /// Returns true if this union contains a value of type B (second case).
        /// </summary>
        public bool Is1 => tag == 1;

        /// <summary>
        /// Returns true if this union contains a value of type C (third case).
        /// </summary>
        public bool Is2 => tag == 2;

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
    }
}