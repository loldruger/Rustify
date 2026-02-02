using System;
using System.Threading;
using System.Threading.Tasks;

using Rustify.Monads;
using Rustify.Interfaces;
using Rustify.Utilities.Synchronizer;

namespace Rustify.Utilities.Sync
{
    public class RwLock<T> : IAsyncRw<T>, IDisposable where T : notnull, IClone<T>
    {
        private readonly SemaphoreSlim semaphore = new(1, 1);
        private T value;
        private bool disposed = false;

        public RwLock(T initialValue)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException(nameof(initialValue), "Initial value cannot be null.");
            }
            this.value = initialValue;
        }

        public Result<T, ISynchronizerError> GetValue()
        {
            if (this.disposed) return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                this.semaphore.Wait();
                try
                {
                    return Result<T, ISynchronizerError>.Ok(this.value.Clone());
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (Exception)
            {
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        public async Task<Result<T, ISynchronizerError>> GetValueAsync(CancellationToken cancellationToken = default)
        {
            if (this.disposed) return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    return Result<T, ISynchronizerError>.Ok(this.value.Clone());
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (ObjectDisposedException)
            {
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (Exception)
            {
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        Task<Result<T, ISynchronizerError>> IAsync<T>.GetValueAsync()
        {
            return this.GetValueAsync(CancellationToken.None);
        }

        public Result<Unit, ISynchronizerError> UpdateValue(Func<T, T> f)
        {
            if (this.disposed) return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                this.semaphore.Wait();
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, ISynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (Exception)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        public async Task<Result<Unit, ISynchronizerError>> UpdateValueAsync(Func<T, T> f, CancellationToken cancellationToken = default)
        {
            if (this.disposed) return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, ISynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (ObjectDisposedException)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (Exception)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        Task<Result<Unit, ISynchronizerError>> IAsync<T>.UpdateValueAsync(Func<T, T> updateFunc)
        {
            return this.UpdateValueAsync(updateFunc, CancellationToken.None);
        }

        public async Task<Result<Result<U, E>, ISynchronizerError>> WithLockedAsync<U, E>(
            Func<T, Task<Result<U, E>>> action,
            CancellationToken cancellationToken = default)
            where U : notnull
            where E : notnull
        {
            if (this.disposed) return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var clonedValue = this.value.Clone();
                    var actionResult = await action(clonedValue).ConfigureAwait(false);
                    return Result<Result<U, E>, ISynchronizerError>.Ok(actionResult);
                }
                finally
                {
                    this.semaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (ObjectDisposedException)
            {
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
            catch (Exception)
            {
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        Task<Result<Result<U, E>, ISynchronizerError>> IAsync<T>.WithLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
        {
            return this.WithLockedAsync(action, CancellationToken.None);
        }

        public async Task<Result<Result<U, E>, ISynchronizerError>> WithReadLockedAsync<U, E>(
            Func<T, Task<Result<U, E>>> action,
            CancellationToken cancellationToken = default)
            where U : notnull
            where E : notnull
        {
            return await this.WithLockedAsync(action, cancellationToken).ConfigureAwait(false);
        }

        Task<Result<Result<U, E>, ISynchronizerError>> IAsyncRw<T>.WithReadLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
        {
            return this.WithReadLockedAsync(action, CancellationToken.None);
        }

        public void Dispose()
        {
            if (this.disposed) return;

            this.disposed = true;
            this.semaphore.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
