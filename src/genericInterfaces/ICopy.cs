namespace Rustify.GenericInterfaces
{
    public interface ICopy<T> where T : notnull
    {
        T Copy();
    }
}