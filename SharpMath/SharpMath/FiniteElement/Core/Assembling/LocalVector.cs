using SharpMath.Vectors;

namespace SharpMath.FiniteElement.Core.Assembling;

public readonly ref struct LocalVector
{
    public double this[int x] => _vector[x];
    public IndexPermutation IndexPermutation { get; }

    private readonly Vector _vector;

    public LocalVector(Vector vector, IndexPermutation permutation)
    {
        IndexPermutation = permutation;
        _vector = vector;
    }
}

public readonly ref struct StackLocalVector
{
    public double this[int x] => _vector[x];
    public StackIndexPermutation IndexPermutation { get; }
    public int Length => _vector.Length;

    private readonly Span<double> _vector;

    public StackLocalVector(Span<double> vector, StackIndexPermutation permutation)
    {
        IndexPermutation = permutation;
        _vector = vector;
    }
}