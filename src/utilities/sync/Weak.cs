using System;
using System.Threading;
using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    /// <summary>
    /// A weak reference to an Arc value. Does not prevent the value from being disposed.
    /// </summary>
    public sealed class Weak<T> where T : notnull
    {
        private readonly WeakReference innerRef;

        internal Weak(ArcInner<T> inner)
        {
            this.innerRef = new WeakReference(inner);
            Interlocked.Increment(ref inner.WeakCount);
        }

        /// <summary>
        /// Attempts to upgrade this weak reference to a strong Arc reference.
        /// Returns None if the value has been disposed.
        /// </summary>
        public Option<Arc<T>> Upgrade()
        {
            var inner = this.innerRef.Target as ArcInner<T>;

            if (inner is null)
                return Option<Arc<T>>.None;

            lock (inner.Lock)
            {
                if (inner.IsDisposed || inner.RefCount == 0)
                    return Option<Arc<T>>.None;

                return Option<Arc<T>>.Some(Arc<T>.FromInner(inner));
            }
        }

        /// <summary>
        /// Returns true if the referenced value is still alive.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                var inner = this.innerRef.Target as ArcInner<T>;
                if (inner is null) return false;
                lock (inner.Lock)
                {
                    return !inner.IsDisposed && inner.RefCount > 0;
                }
            }
        }

        /// <summary>
        /// Gets the current weak reference count.
        /// </summary>
        public int WeakCount()
        {
            var inner = this.innerRef.Target as ArcInner<T>;
            if (inner is null) return 0;
            return Volatile.Read(ref inner.WeakCount);
        }
    }
}
