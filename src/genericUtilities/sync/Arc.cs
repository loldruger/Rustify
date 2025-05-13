using System;
using System.Threading;
using Rustify.GenericInterfaces;
using Rustify.Monads;

namespace Rustify.GenericUtilities.Sync
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
            // Dispose를 호출하면 참조 카운트 감소
            this.Release();

            // 필요시 Dispose 호출된 것을 GC에 알림
            GC.SuppressFinalize(this);
        }
    }
}