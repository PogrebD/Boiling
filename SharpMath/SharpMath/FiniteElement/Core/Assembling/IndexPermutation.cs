namespace SharpMath.FiniteElement.Core.Assembling;

public readonly struct IndexPermutation
{
    public int Apply(int index) => _permutation[index];
    public int Length => _permutation.Length;

    private readonly int[] _permutation;

    public IndexPermutation(int[] permutation)
    {
        _permutation = permutation;
    }
}

public readonly ref struct StackIndexPermutation
{
    public int Apply(int index) => Permutation[index];
    public int Length => Permutation.Length;
    public readonly Span<int> Permutation;

    public StackIndexPermutation(Span<int> permutation)
    {
        Permutation = permutation;
    }
}