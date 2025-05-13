using Rustify.Monads;

namespace Rustify.Utilities.Synchronizer
{
    public interface IAsync<T> : ISynchronizer<T> where T : notnull
    {
        Task<Result<T, ISynchronizerError>> GetValueAsync();

        Task<Result<Result<U, E>, ISynchronizerError>> WithLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull;

        Task<Result<Unit, ISynchronizerError>> UpdateValueAsync(Func<T, T> updateFunc);
    }
}