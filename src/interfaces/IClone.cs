namespace Rustify.Interfaces
{
    public interface IClone<T> where T : notnull
    {
        T Clone();
    }
}