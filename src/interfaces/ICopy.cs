namespace Rustify.Interfaces
{
    public interface ICopy<T> where T : notnull
    {
        T Copy();
    }
}