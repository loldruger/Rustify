using Rustify.Monads;

namespace Rustify.Utilities.Synchronizer
{
    public interface IAsyncRw<T> : IAsync<T> where T : notnull
    {
        Task<Result<Result<U, E>, ISynchronizerError>> WithReadLockedAsync<U, E>(Func<T, Task<Result<U, E>>> action)
            where U : notnull
            where E : notnull;
    }
}