using Rustify.Monads;

namespace Rustify.Utilities.Synchronizer
{
    public enum ISynchronizerError
    {
        Locked,
        Failed,
        UnknownError,
        RecursionError
    }

    public interface ISynchronizer<T> where T : notnull
    {
        Result<T, ISynchronizerError> GetValue();
    }
}