using SharpMath.Matrices.Sparse.Storages;

namespace SharpMath.Vectors;

public interface IReadonlyVector<T> : IEnumerable<IndexValue<T>>
{
    public T this[int x] { get; }
    public int Length { get; }
    public T Norm { get; }

    public T ScalarProduct(IReadonlyVector<T> v);
}

public interface IReadonlyVector : IReadonlyVector<double> 
    {}