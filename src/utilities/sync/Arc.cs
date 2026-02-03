using System;
using System.Threading;
using Rustify.Interfaces;
using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    /// <summary>
    /// Arc&lt;T&gt; (Atomic Reference Counting) provides thread-safe shared ownership of a value.
    /// The value is deallocated only when the last Arc pointer to it is released.
    /// </summary>
    /// <typeparam name="T">The type of value to be shared. Must not be null.</typeparam>
    public class Arc<T> : IClone<Arc<T>>, IDisposable where T : notnull
    {
        private Option<T> value;
        private int count = 1;
        private readonly object _lock = new();
        private volatile bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the Arc&lt;T&gt; class with the specified value.
        /// </summary>
        /// <param name="value">The value to be shared across Arc instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        public Arc(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.value = Option.Some(value);
        }

        /// <summary>
        /// Creates a new Arc&lt;T&gt; instance wrapping the specified value.
        /// This is a static factory method that follows Rust conventions.
        /// </summary>
        /// <param name="value">The value to wrap in an Arc.</param>
        /// <returns>A new Arc&lt;T&gt; instance.</returns>
        public static Arc<T> New(T value) => new Arc<T>(value);

        /// <summary>
        /// Decrements the reference count. When the count reaches zero, the contained value is disposed if it implements IDisposable.
        /// </summary>
        /// <returns>The new reference count after decrementing.</returns>
        public int Release()
        {
            lock (this._lock)
            {
                if (this.isDisposed)
                {
                    return this.count;
                }

                // Prevent count from going negative
                if (this.count <= 0)
                {
                    return this.count;
                }

                // Simple decrement inside lock - no need for Interlocked
                this.count--;

                if (this.count == 0)
                {
                    if (this.value.IsSome())
                    {
                        T actualValue = this.value.Unwrap();
                        if (actualValue is IDisposable disposableValue)
                        {
                            disposableValue.Dispose();
                        }
                    }
                    this.value = Option.None<T>();
                    this.isDisposed = true;
                }
                return this.count;
            }
        }

        /// <summary>
        /// Gets the current strong reference count.
        /// </summary>
        /// <returns>The number of active Arc references to this value.</returns>
        public int StrongCount()
        {
            lock (this._lock)
            {
                return this.count;
            }
        }

        /// <summary>
        /// Retrieves the shared value.
        /// </summary>
        /// <returns>The contained value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Arc has been disposed.</exception>
        public T GetValue()
        {
            lock (this._lock)
            {
                if (this.count <= 0 || this.isDisposed)
                {
                    throw new InvalidOperationException("Object is disposed.");
                }

                return this.value.Unwrap();
            }
        }

        /// <summary>
        /// Alias for GetValue(). Retrieves the shared value with lock semantics.
        /// </summary>
        /// <returns>The contained value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Arc has been disposed.</exception>
        public T Lock() => GetValue();

        /// <summary>
        /// Creates a new Arc instance that shares ownership of the same value.
        /// This increments the reference count.
        /// </summary>
        /// <returns>A new Arc instance pointing to the same value.</returns>
        /// <exception cref="InvalidOperationException">Thrown when attempting to clone a disposed Arc.</exception>
        public Arc<T> Clone()
        {
            lock (this._lock)
            {
                if (this.count <= 0 || this.isDisposed)
                {
                    throw new InvalidOperationException("Cannot clone a disposed object.");
                }

                // Simple increment inside lock - no need for Interlocked
                this.count++;

                return this;
            }
        }

        /// <summary>
        /// Releases the Arc instance and decrements the reference count.
        /// </summary>
        public void Dispose()
        {
            this.Release();
            GC.SuppressFinalize(this);
        }
    }
}