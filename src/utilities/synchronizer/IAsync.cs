using System;
using System.Threading.Tasks;

using Rustify.Monads;

namespace Rustify.Utilities.Synchronizer
{
    public interface IAsync<T> : ISynchronizer<T> where T : notnull
    {
        Task<Result<T, SynchronizerError>> GetValueAsync();

        Task<Result<Result<U, E>, SynchronizerError>> WithLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull;

        Task<Result<Unit, SynchronizerError>> UpdateValueAsync(Func<T, T> updateFunc);
    }
}