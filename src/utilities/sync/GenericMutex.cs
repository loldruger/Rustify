using System;
using System.Threading;
using System.Threading.Tasks;

using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    public enum GenericMutexError
    {
        MutexLocked,
        MutexFailed,
        UnknownError
    }

    public class GenericMutex<T> : IDisposable where T : notnull
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private T value;
        private int disposed = 0;

        public GenericMutex(T value)
        {
            this.value = value;
        }

        public Result<T, GenericMutexError> TryGetValue()
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            if (this.semaphore.Wait(0))
            {
                try
                {
                    return Result<T, GenericMutexError>.Ok(this.value);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }

            return Result<T, GenericMutexError>.Err(GenericMutexError.MutexLocked);
        }

        public async Task<Result<T, GenericMutexError>> TryGetValueAsync(CancellationToken cancellationToken = default)
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                if (await this.semaphore.WaitAsync(0, cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        return Result<T, GenericMutexError>.Ok(this.value);
                    }
                    finally
                    {
                        this.semaphore.Release();
                    }
                }

                return Result<T, GenericMutexError>.Err(GenericMutexError.MutexLocked);
            }
            catch (OperationCanceledException)
            {
                return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public Result<T, GenericMutexError> GetValue()
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                this.semaphore.Wait();
                try
                {
                    return Result<T, GenericMutexError>.Ok(this.value);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public async Task<Result<T, GenericMutexError>> GetValueAsync(CancellationToken cancellationToken = default)
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    return Result<T, GenericMutexError>.Ok(this.value);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (ObjectDisposedException)
            {
                return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public Result<Result<U, E>, GenericMutexError> WithLock<U, E>(Func<T, Result<U, E>> action)
            where U : notnull
            where E : notnull
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                this.semaphore.Wait();
                try
                {
                    return Result<Result<U, E>, GenericMutexError>.Ok(action(this.value));
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (Exception)
            {
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public async Task<Result<Result<U, E>, GenericMutexError>> WithLockAsync<U, E>(
            Func<T, Task<Result<U, E>>> action,
            CancellationToken cancellationToken = default)
            where U : notnull
            where E : notnull
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    return Result<Result<U, E>, GenericMutexError>.Ok(await action(this.value).ConfigureAwait(false));
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (ObjectDisposedException)
            {
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (Exception)
            {
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public Result<Unit, GenericMutexError> UpdateValue(Func<T, T> f)
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                this.semaphore.Wait();
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, GenericMutexError>.Ok(Unit.New);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (Exception)
            {
                return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public async Task<Result<Unit, GenericMutexError>> UpdateValueAsync(
            Func<T, T> f,
            CancellationToken cancellationToken = default)
        {
            if (Volatile.Read(ref this.disposed) != 0) return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, GenericMutexError>.Ok(Unit.New);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            catch (Exception)
            {
                return Result<Unit, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) == 1) return;

            this.semaphore.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
