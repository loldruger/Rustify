using System;

namespace Rustify.Utilities
{
    public static class TaggedUnion
    {
        public static TaggedUnion<A> _0<A>(A a)
            where A : notnull
        {
            return TaggedUnion<A>.__0(a);
        }
    }

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

        public static TaggedUnion<A> __0(A b) => new(0, b);
        public static implicit operator TaggedUnion<A>(A a) => __0(a);

        public R Match<R>(System.Func<A, R> _0)
        {
            return this.tag switch
            {
                0 => _0(this._0),
                _ => throw new InvalidOperationException("Invalid tag value")
            };
        }
    }

}