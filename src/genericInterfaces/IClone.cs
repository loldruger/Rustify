namespace Rustify.GenericInterfaces
{
    public interface IClone<T> where T : notnull
    {
        T Clone();
    }
}