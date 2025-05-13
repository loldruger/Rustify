using System;
using System.Threading;
using System.Threading.Tasks;
using Rustify.Monads;

namespace Rustify.GenericUtilities.Sync
{
    public enum GenericMutexError
    {
        MutexLocked,
        MutexFailed,
        UnknownError
    }

    public partial class GenericMutex<T>(T value) where T : notnull
    {
        readonly Mutex mutex = new();
        private T value = value;

        /// <summary>
        /// Attempts to lock the mutex and retrieve the value.
        /// without blocking if the mutex is already locked.
        /// </summary>
        public Result<T, GenericMutexError> TryGetValue()
        {
            if (this.mutex.WaitOne(0)) // Changed from TryLock()
            {
                try
                {
                    return Result<T, GenericMutexError>.Ok(this.value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Mutex operation failed: {ex.Message}");
                    return Result<T, GenericMutexError>.Err(GenericMutexError.MutexFailed);
                }
                finally
                {
                    this.mutex.ReleaseMutex(); // Changed from Unlock()
                }
            }

            return Result<T, GenericMutexError>.Err(GenericMutexError.MutexLocked);
        }

        /// <summary>
        /// Calls the provided action with the value of the mutex, ensuring that the mutex is locked during the call.
        /// </summary>
        public Result<Result<U, E>, GenericMutexError> WithLock<U, E>(Func<T, Result<U, E>> action)
            where U : notnull
            where E : notnull
        {
            this.mutex.WaitOne(); // Changed from Lock()
            try
            {
                return Result<Result<U, E>, GenericMutexError>.Ok(action(this.value));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mutex operation failed: {ex.Message}");
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            finally
            {
                this.mutex.ReleaseMutex(); // Changed from Unlock()
            }
        }

        /// <summary>
        /// Calls the provided action with the value of the mutex, ensuring that the mutex is locked during the call.
        /// </summary>
        public async Task<Result<Result<U, E>, GenericMutexError>> WithLockAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull
        {
            await Task.Run(() => this.mutex.WaitOne()); // Changed from Lock() and wrapped in Task.Run for async
            try
            {
                return Result<Result<U, E>, GenericMutexError>.Ok(await action(this.value));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mutex operation failed: {ex.Message}");
                return Result<Result<U, E>, GenericMutexError>.Err(GenericMutexError.MutexFailed);
            }
            finally
            {
                this.mutex.ReleaseMutex(); // Changed from Unlock()
            }
        }

        /// <summary>
        /// Updates the value of the mutex using the provided function, ensuring that the mutex is locked during the update.
        /// </summary>
        public Unit UpdateValue(Func<T, T> f)
        {
            this.mutex.WaitOne(); // Changed from Lock()
            try
            {
                this.value = f(this.value);
                return Unit.New;
            }
            finally
            {
                this.mutex.ReleaseMutex(); // Changed from Unlock()
            }
        }
    }
}