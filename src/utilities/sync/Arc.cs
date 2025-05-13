using System;
using System.Threading;
using Rustify.Interfaces;
using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    public class Arc<T> : IClone<Arc<T>>, IDisposable where T : notnull
    {
        private Option<T> value;
        private int count = 1;
        private readonly object _lock = new();
        private bool isDisposed = false;

        public Arc(T value)
        {
            this.value = Option.Some(value);
        }

        public int Release()
        {
            lock (this._lock)
            {
                if (this.isDisposed)
                {
                    return this.count;
                }

                int newCount = Interlocked.Decrement(ref this.count);
                if (newCount == 0)
                {
                    if (this.value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                    this.value = Option.None<T>();
                    this.isDisposed = true;
                }
                return newCount;
            }
        }

        public T GetValue()
        {
            lock (this._lock)
            {
                if (this.count <= 0 || this.isDisposed)
                {
                    throw new InvalidOperationException("Object is disposed.");
                }
                else
                {
                    return this.value.Unwrap();
                }
            }
        }

        public Arc<T> Clone()
        {
            lock (this._lock)
            {
                if (this.count <= 0 || this.isDisposed)
                {
                    throw new InvalidOperationException("Cannot clone a disposed object.");
                }

                Interlocked.Increment(ref this.count);

                return this;
            }
        }

        public void Dispose()
        {
            this.Release();

            GC.SuppressFinalize(this);
        }
    }
}