using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling;

public class LocalMatrix
{
    public double this[int x, int y] => Matrix[x, y];
    public IndexPermutation IndexPermutation { get; }
    public ImmutableMatrix Matrix { get; private set; }

    public LocalMatrix(ImmutableMatrix matrix, IndexPermutation indexPermutation)
    {
        Matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}

public readonly ref struct StackLocalMatrix
{
    public double this[int x, int y] => Matrix[x, y];
    public StackIndexPermutation IndexPermutation { get; }
    public StackMatrix Matrix { get; }

    public StackLocalMatrix(StackMatrix matrix, StackIndexPermutation indexPermutation)
    {
        Matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}