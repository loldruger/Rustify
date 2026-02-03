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
        private readonly SemaphoreSlim writeLock = new(1, 1);
        private readonly SemaphoreSlim readerCountLock = new(1, 1);
        private int readerCount = 0;
        private T value;
        private int disposed = 0;

        public RwLock(T initialValue)
        {
            if (initialValue == null)
            {
                throw new ArgumentNullException(nameof(initialValue), "Initial value cannot be null.");
            }
            this.value = initialValue;
        }

        private async Task AcquireReadLockAsync(CancellationToken cancellationToken)
        {
            await this.readerCountLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                this.readerCount++;
                if (this.readerCount == 1)
                {
                    try
                    {
                        await this.writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch
                    {
                        this.readerCount--;
                        throw;
                    }
                }
            }
            finally
            {
                this.readerCountLock.Release();
            }
        }

        private async Task ReleaseReadLockAsync()
        {
            await this.readerCountLock.WaitAsync().ConfigureAwait(false);
            try
            {
                this.readerCount--;
                if (this.readerCount == 0)
                {
                    this.writeLock.Release();
                }
            }
            finally
            {
                this.readerCountLock.Release();
            }
        }

        private void AcquireReadLock()
        {
            this.readerCountLock.Wait();
            try
            {
                this.readerCount+=1;
                if (this.readerCount == 1)
                {
                    try
                    {
                        this.writeLock.Wait();
                    }
                    catch
                    {
                        this.readerCount--;
                        throw;
                    }
                }
            }
            finally
            {
                this.readerCountLock.Release();
            }
        }

        private void ReleaseReadLock()
        {
            this.readerCountLock.Wait();
            try
            {
                this.readerCount-=1;
                if (this.readerCount == 0)
                {
                    this.writeLock.Release();
                }
            }
            finally
            {
                this.readerCountLock.Release();
            }
        }

        public Result<T, ISynchronizerError> GetValue()
        {
            if (this.disposed != 0) return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                this.AcquireReadLock();
                try
                {
                    return Result<T, ISynchronizerError>.Ok(this.value.Clone());
                }
                finally
                {
                    this.ReleaseReadLock();
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
            if (this.disposed != 0) return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.AcquireReadLockAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    return Result<T, ISynchronizerError>.Ok(this.value.Clone());
                }
                finally
                {
                    await this.ReleaseReadLockAsync().ConfigureAwait(false);
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
            if (this.disposed != 0) return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                this.writeLock.Wait();
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, ISynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.writeLock.Release();
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
            if (this.disposed != 0) return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    this.value = f(this.value);
                    return Result<Unit, ISynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    this.writeLock.Release();
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
            if (this.disposed != 0) return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var actionResult = await action(this.value).ConfigureAwait(false);
                    return Result<Result<U, E>, ISynchronizerError>.Ok(actionResult);
                }
                finally
                {
                    this.writeLock.Release();
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
            if (this.disposed != 0) return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);

            try
            {
                await this.AcquireReadLockAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var clonedValue = this.value.Clone();
                    var actionResult = await action(clonedValue).ConfigureAwait(false);
                    return Result<Result<U, E>, ISynchronizerError>.Ok(actionResult);
                }
                finally
                {
                    await this.ReleaseReadLockAsync().ConfigureAwait(false);
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

        Task<Result<Result<U, E>, ISynchronizerError>> IAsyncRw<T>.WithReadLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
        {
            return this.WithReadLockedAsync(action, CancellationToken.None);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) == 1) return;

            this.writeLock.Dispose();
            this.readerCountLock.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
