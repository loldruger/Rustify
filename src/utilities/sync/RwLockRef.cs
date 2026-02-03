using System;
using System.Threading;
using System.Threading.Tasks;
using Rustify.Monads;
using Rustify.Utilities.Synchronizer;

namespace Rustify.Utilities.Sync
{
    /// <summary>
    /// A read-write lock for reference types that provides callback-based access.
    /// Unlike RwLock&lt;T&gt;, this does not require IClone&lt;T&gt; and does not expose GetValue.
    /// Access is always through WithRead or WithWrite callbacks for safety.
    /// </summary>
    public sealed class RwLockRef<T> : IDisposable where T : class
    {
        private readonly ReaderWriterLockSlim rwLock;
        private T value;
        private int disposed = 0;

        public RwLockRef(T value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            this.rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        /// Executes a read operation on the locked value.
        /// Multiple readers can access simultaneously.
        /// </summary>
        public Result<U, SynchronizerError> WithRead<U>(Func<T, U> reader) where U : notnull
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (Volatile.Read(ref this.disposed) != 0)
                return Result<U, SynchronizerError>.Err(SynchronizerError.Disposed());

            try
            {
                this.rwLock.EnterReadLock();
                try
                {
                    return Result<U, SynchronizerError>.Ok(reader(this.value));
                }
                finally
                {
                    this.rwLock.ExitReadLock();
                }
            }
            catch (LockRecursionException)
            {
                return Result<U, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<U, SynchronizerError>.Err(SynchronizerError.Disposed());
            }
        }

        /// <summary>
        /// Executes a write operation that transforms the locked value.
        /// Exclusive access - blocks all other readers and writers.
        /// </summary>
        public Result<Unit, SynchronizerError> WithWrite(Func<T, T> writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (Volatile.Read(ref this.disposed) != 0)
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());

            try
            {
                this.rwLock.EnterWriteLock();
                try
                {
                    this.value = writer(this.value);
                    return Result<Unit, SynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.rwLock.ExitWriteLock();
                }
            }
            catch (LockRecursionException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());
            }
        }

        /// <summary>
        /// Executes a write operation that mutates the value in place.
        /// For when you need to modify the object without replacing it.
        /// </summary>
        public Result<Unit, SynchronizerError> WithWriteMutate(Action<T> mutator)
        {
            if (mutator == null) throw new ArgumentNullException(nameof(mutator));
            if (Volatile.Read(ref this.disposed) != 0)
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());

            try
            {
                this.rwLock.EnterWriteLock();
                try
                {
                    mutator(this.value);
                    return Result<Unit, SynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.rwLock.ExitWriteLock();
                }
            }
            catch (LockRecursionException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());
            }
        }

        /// <summary>
        /// Async version of WithRead. Captures the value under lock, then executes the async callback outside of lock.
        /// </summary>
        public async Task<Result<U, SynchronizerError>> WithReadAsync<U>(
            Func<T, Task<U>> reader,
            CancellationToken cancellationToken = default) where U : notnull
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (Volatile.Read(ref this.disposed) != 0)
                return Result<U, SynchronizerError>.Err(SynchronizerError.Disposed());

            if (cancellationToken.IsCancellationRequested)
                return Result<U, SynchronizerError>.Err(SynchronizerError.Cancelled());

            T capturedValue;
            try
            {
                this.rwLock.EnterReadLock();
                try
                {
                    capturedValue = this.value;
                }
                finally
                {
                    this.rwLock.ExitReadLock();
                }
            }
            catch (LockRecursionException)
            {
                return Result<U, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<U, SynchronizerError>.Err(SynchronizerError.Disposed());
            }

            try
            {
                var result = await reader(capturedValue).ConfigureAwait(false);
                return Result<U, SynchronizerError>.Ok(result);
            }
            catch (OperationCanceledException)
            {
                return Result<U, SynchronizerError>.Err(SynchronizerError.Cancelled());
            }
        }

        /// <summary>
        /// Async version of WithWrite. Acquires write lock, executes async callback, updates value, releases lock.
        /// </summary>
        public async Task<Result<Unit, SynchronizerError>> WithWriteAsync(
            Func<T, Task<T>> writer,
            CancellationToken cancellationToken = default)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (Volatile.Read(ref this.disposed) != 0)
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());

            if (cancellationToken.IsCancellationRequested)
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Cancelled());

            T capturedValue;
            try
            {
                this.rwLock.EnterWriteLock();
                try
                {
                    capturedValue = this.value;
                }
                finally
                {
                    this.rwLock.ExitWriteLock();
                }
            }
            catch (LockRecursionException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());
            }

            try
            {
                var newValue = await writer(capturedValue).ConfigureAwait(false);
                
                this.rwLock.EnterWriteLock();
                try
                {
                    this.value = newValue;
                    return Result<Unit, SynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.rwLock.ExitWriteLock();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Cancelled());
            }
            catch (LockRecursionException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.RecursionError());
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, SynchronizerError>.Err(SynchronizerError.Disposed());
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) == 1) return;

            this.rwLock.Dispose();

            if (this.value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
