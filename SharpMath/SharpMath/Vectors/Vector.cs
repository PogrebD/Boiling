using System.Collections;
using SharpMath.Matrices.Sparse.Storages;

namespace SharpMath.Vectors;

public class Vector : IVector<double>
{
    public static Vector Create(int length, double defaultValue = 0)
    {
        return Create(length, _ => defaultValue);
    }
    public static Vector Create(int length, Func<int, double> filling)
    {
        var values = new double[length];
        for (int i = 0; i < length; i++)
            values[i] = filling(i);

        return new Vector(values);
    }

    public virtual double this[int x]
    {
        get => _values[x];
        set => _values[x] = value;
    }
    public int Length => _values.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    private readonly double[] _values;

    public Vector Copy()
    {
        var values = new double[Length];
        return CopyTo(values);
    }

    public TVector CopyTo<TVector>(TVector memory) where TVector : IVector<double>
    {
        if (memory == null || memory.Length != Length)
            throw new ArgumentException();

        for (var i = 0; i < Length; i++)
            memory[i] = this[i];

        return memory;
    }

    public Vector CopyTo(double[] memory)
    {
        if (memory == null || memory.Length != Length)
            throw new ArgumentException();

        for (var i = 0; i < Length; i++)
            memory[i] = this[i];

        return new Vector(memory);
    }

    public Vector(params double[] values)
    {
        _values = values;
    }

    public static double ScalarProduct(IReadonlyVector<double> v, IReadonlyVector<double> u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)} must have the same length");

        var sum = 0d;

        for (var i = 0; i < v.Length; i++)
            sum += u[i] * v[i];

        return sum;
    }

    public double ScalarProduct(IReadonlyVector<double> v)
    {
        return ScalarProduct(this, v);
    }

    public IEnumerator<IndexValue<double>> GetEnumerator()
    {
        for (var i = 0; i < Length; i++)
            yield return new IndexValue<double>(this[i], i);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}