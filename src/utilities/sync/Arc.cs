using System;
using System.Threading;
using Rustify.Interfaces;
using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    /// <summary>
    /// Internal shared state for Arc instances.
    /// </summary>
    internal sealed class ArcInner<T> where T : notnull
    {
        public T Value { get; }
        public int RefCount;
        public int WeakCount;
        public bool IsDisposed;
        public readonly object Lock = new();

        public ArcInner(T value)
        {
            this.Value = value;
            this.RefCount = 0;
            this.WeakCount = 0;
            this.IsDisposed = false;
        }
    }

    /// <summary>
    /// Arc&lt;T&gt; (Atomic Reference Counting) provides thread-safe shared ownership of a value.
    /// Each Arc instance represents a single ownership handle. When all handles are disposed,
    /// the underlying value is deallocated.
    /// </summary>
    /// <typeparam name="T">The type of value to be shared. Must not be null.</typeparam>
    public sealed class Arc<T> : IClone<Arc<T>>, IDisposable where T : notnull
    {
        private readonly ArcInner<T> inner;
        private bool released = false;

        private Arc(ArcInner<T> inner)
        {
            this.inner = inner;
            lock (inner.Lock)
            {
                inner.RefCount++;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Arc&lt;T&gt; class with the specified value.
        /// </summary>
        /// <param name="value">The value to be shared across Arc instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        public Arc(T value) : this(new ArcInner<T>(value ?? throw new ArgumentNullException(nameof(value)))) { }

        /// <summary>
        /// Creates a new Arc&lt;T&gt; instance wrapping the specified value.
        /// This is a static factory method that follows Rust conventions.
        /// </summary>
        /// <param name="value">The value to wrap in an Arc.</param>
        /// <returns>A new Arc&lt;T&gt; instance.</returns>
        public static Arc<T> New(T value) => new(value);

        /// <summary>
        /// Gets the current strong reference count.
        /// </summary>
        /// <returns>The number of active Arc references to this value.</returns>
        public int StrongCount()
        {
            lock (this.inner.Lock)
            {
                return this.inner.RefCount;
            }
        }

        /// <summary>
        /// Retrieves the shared value.
        /// </summary>
        /// <returns>The contained value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Arc has been disposed.</exception>
        public T GetValue()
        {
            lock (this.inner.Lock)
            {
                if (this.released || this.inner.IsDisposed)
                {
                    throw new InvalidOperationException("Arc is disposed.");
                }
                return this.inner.Value;
            }
        }

        /// <summary>
        /// Alias for GetValue(). Retrieves the shared value with lock semantics.
        /// </summary>
        /// <returns>The contained value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Arc has been disposed.</exception>
        public T Lock() => this.GetValue();

        /// <summary>
        /// Creates a new Arc instance that shares ownership of the same value.
        /// This is a true clone - a new handle with its own lifetime.
        /// </summary>
        /// <returns>A new Arc instance pointing to the same value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to clone a disposed Arc.</exception>
        public Arc<T> Clone()
        {
            lock (this.inner.Lock)
            {
                if (this.released || this.inner.IsDisposed)
                {
                    throw new InvalidOperationException("Cannot clone a disposed Arc.");
                }
                return new Arc<T>(this.inner);
            }
        }

        /// <summary>
        /// Creates a weak reference to this Arc.
        /// </summary>
        /// <returns>A new Weak reference that does not prevent disposal.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Arc has been disposed.</exception>
        public Weak<T> Downgrade()
        {
            lock (this.inner.Lock)
            {
                if (this.released || this.inner.IsDisposed)
                {
                    throw new InvalidOperationException("Cannot downgrade a disposed Arc.");
                }
                return new Weak<T>(this.inner);
            }
        }

        /// <summary>
        /// Creates an Arc from an existing inner reference (used by Weak.Upgrade).
        /// </summary>
        /// <param name="inner">The inner reference to wrap.</param>
        /// <returns>A new Arc instance.</returns>
        internal static Arc<T> FromInner(ArcInner<T> inner) => new(inner);

        /// <summary>
        /// Releases this Arc instance and decrements the reference count.
        /// When the count reaches zero, the contained value is disposed if it implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            lock (this.inner.Lock)
            {
                if (this.released) return;
                this.released = true;

                this.inner.RefCount--;
                if (this.inner.RefCount == 0 && !this.inner.IsDisposed)
                {
                    this.inner.IsDisposed = true;
                    if (this.inner.Value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
