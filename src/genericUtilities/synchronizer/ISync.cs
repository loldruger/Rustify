using System;
using Rustify.Monads;

namespace Rustify.GenericUtilities.Synchronizer
{
    public interface ISync<T> : ISynchronizer<T> where T : notnull
    {
        Result<Unit, ISynchronizerError> Lock();
        Result<Unit, ISynchronizerError> Unlock();

        /// <summary>
        /// Calls the provided action with the value of the mutex, ensuring that the mutex is locked during the call.
        /// </summary>
        Result<Result<U, E>, ISynchronizerError> WithLocked<U, E>(Func<T, Result<U, E>> action)
            where U : notnull
            where E : notnull;
    }
}