using Rustify.Interfaces;
using Rustify.Utilities.Synchronizer;
using Rustify.Monads;

namespace Rustify.Utilities.Sync
{
    /// <summary>
    /// Provides asynchronous read-write lock functionality around a value of type T.
    /// Uses ReaderWriterLockSlim internally.
    /// Note: Requires T to implement IClone<T> to safely provide snapshots of the value
    /// during asynchronous operations, minimizing lock duration.
    /// </summary>
    public class RwLock<T> : IAsyncRw<T> where T : notnull, IClone<T>
    {
        private readonly ReaderWriterLockSlim rwLock = new();
        private T value;

        public RwLock(T initialValue)
        {
            // Ensure initial value is not null according to coding instructions
            if (initialValue == null)
            {
                // This should ideally not happen if called correctly, but added for safety.
                // Consider throwing or returning an initialization error if null is possible.
                throw new ArgumentNullException(nameof(initialValue), "Initial value cannot be null.");
            }
            this.value = initialValue;
        }

        /// <summary>
        /// Synchronously gets a clone of the current value with a read lock.
        /// </summary>
        public Result<T, ISynchronizerError> GetValue()
        {
            try
            {
                this.rwLock.EnterReadLock();
                try
                {
                    // Return a clone to prevent external modification after lock release
                    return Result<T, ISynchronizerError>.Ok(this.value.Clone());
                }
                finally
                {
                    // Ensure the lock is held before trying to exit
                    if (this.rwLock.IsReadLockHeld)
                    {
                        this.rwLock.ExitReadLock();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RwLock.GetValue failed: {ex.Message}");
                // Return a generic failure error
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        /// <summary>
        /// Asynchronously gets a clone of the current value with a read lock.
        /// Ensures lock acquisition and release happen on the same thread via Task.Run.
        /// </summary>
        public async Task<Result<T, ISynchronizerError>> GetValueAsync()
        {
            try
            {
                // Perform lock acquisition, value cloning, and lock release within Task.Run
                // to ensure thread affinity for ReaderWriterLockSlim.
                T clonedValue = await Task.Run(() =>
                {
                    this.rwLock.EnterReadLock();
                    try
                    {
                        // Clone the value while the lock is held
                        return this.value.Clone();
                    }
                    finally
                    {
                        // Ensure the lock is released on the same thread it was acquired
                        if (this.rwLock.IsReadLockHeld)
                        {
                            this.rwLock.ExitReadLock();
                        }
                    }
                });
                // Return the cloned value after the lock is released
                return Result<T, ISynchronizerError>.Ok(clonedValue);
            }
            catch (LockRecursionException) // Catch specific lock recursion error
            {
                // Return a specific error for lock recursion
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.RecursionError);
            }
            catch (Exception ex) // Catch any other exceptions, including those from Task.Run
            {
                Console.WriteLine($"GetValueAsync failed: {ex}");
                // Return a generic failure error
                return Result<T, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        /// <summary>
        /// Asynchronously updates the value using the provided function `f` with a write lock.
        /// Ensures lock acquisition and release happen on the same thread via Task.Run.
        /// Note: This version does not handle errors within the update function `f`.
        /// Consider a version accepting Func<T, Task<Result<T, E>>> for more robust error handling.
        /// </summary>
        public async Task<Result<Unit, ISynchronizerError>> UpdateValueAsync(Func<T, T> f)
        {
            try
            {
                // Perform lock acquisition, value update, and lock release within Task.Run.
                await Task.Run(() =>
                {
                    this.rwLock.EnterWriteLock();
                    try
                    {
                        // Apply the update function to the value
                        this.value = f(this.value);
                    }
                    finally
                    {
                        // Ensure the lock is released on the same thread
                        if (this.rwLock.IsWriteLockHeld)
                        {
                            this.rwLock.ExitWriteLock();
                        }
                    }
                });
                // Return success if the operation completed without exceptions
                return Result<Unit, ISynchronizerError>.Ok(Unit.New);
            }
            catch (LockRecursionException)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.RecursionError);
            }
            catch (Exception ex) // Catches exceptions from Task.Run or the update function f
            {
                Console.WriteLine($"UpdateValueAsync failed: {ex}");
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        /// <summary>
        /// Synchronously updates the value using the provided function `f` with a write lock.
        /// Note: This version does not handle errors within the update function `f`.
        /// </summary>
        public Result<Unit, ISynchronizerError> UpdateValue(Func<T, T> f)
        {
            try
            {
                this.rwLock.EnterWriteLock();
                try
                {
                    // Apply the update function
                    this.value = f(this.value);
                    // Return success
                    return Result<Unit, ISynchronizerError>.Ok(Unit.New);
                }
                finally
                {
                    // Ensure the lock is held before trying to exit
                    if (this.rwLock.IsWriteLockHeld)
                    {
                        this.rwLock.ExitWriteLock();
                    }
                }
            }
            catch (LockRecursionException)
            {
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.RecursionError);
            }
            catch (Exception ex) // Catches exceptions from the update function f
            {
                Console.WriteLine($"UpdateValue failed: {ex}");
                return Result<Unit, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }
        }

        /// <summary>
        /// Executes an asynchronous action with a write lock held.
        /// Provides a clone of the value to the action to minimize lock duration.
        /// </summary>
        public async Task<Result<Result<U, E>, ISynchronizerError>> WithLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull
        {
            T localValueClone;
            try
            {
                // Acquire write lock, clone value, release lock within Task.Run
                localValueClone = await Task.Run(() =>
                {
                    this.rwLock.EnterWriteLock();
                    try
                    {
                        return this.value.Clone();
                    }
                    finally
                    {
                        if (this.rwLock.IsWriteLockHeld)
                        {
                            this.rwLock.ExitWriteLock();
                        }
                    }
                });
            }
            catch (Exception ex) // Catches exceptions from Task.Run during lock/clone
            {
                Console.WriteLine($"WithLockedAsync failed during lock acquisition/clone: {ex}");
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }

            // Execute the user action outside the lock with the cloned value
            Result<U, E> actionResult;
            try
            {
                actionResult = await action(localValueClone);
            }
            catch (Exception ex) // Catches exceptions from the user-provided action
            {
                Console.WriteLine($"WithLockedAsync action failed: {ex}");
                // Wrap the action's potential failure in the outer Result structure
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }

            // Wrap the successful action result
            return Result<Result<U, E>, ISynchronizerError>.Ok(actionResult);
        }

        /// <summary>
        /// Executes an asynchronous action with a read lock held.
        /// Provides a clone of the value to the action to minimize lock duration.
        /// </summary>
        public async Task<Result<Result<U, E>, ISynchronizerError>> WithReadLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull
        {
            T localValueClone;
            try
            {
                // Acquire read lock, clone value, release lock within Task.Run
                localValueClone = await Task.Run(() =>
                {
                    this.rwLock.EnterReadLock();
                    try
                    {
                        return this.value.Clone();
                    }
                    finally
                    {
                        if (this.rwLock.IsReadLockHeld)
                        {
                            this.rwLock.ExitReadLock();
                        }
                    }
                });
            }
            catch (Exception ex) // Catches exceptions from Task.Run during lock/clone
            {
                Console.WriteLine($"WithReadLockedAsync failed during lock acquisition/clone: {ex}");
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }

            // Execute the user action outside the lock with the cloned value
            Result<U, E> actionResult;
            try
            {
                actionResult = await action(localValueClone);
            }
            catch (Exception ex) // Catches exceptions from the user-provided action
            {
                Console.WriteLine($"WithReadLockedAsync action failed: {ex}");
                return Result<Result<U, E>, ISynchronizerError>.Err(ISynchronizerError.Failed);
            }

            // Wrap the successful action result
            return Result<Result<U, E>, ISynchronizerError>.Ok(actionResult);
        }
    }
}