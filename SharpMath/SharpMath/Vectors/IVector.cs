namespace SharpMath.Vectors;

public interface IVector<T> : IReadonlyVector<T>
{
    public T this[int x] { set; }
}
