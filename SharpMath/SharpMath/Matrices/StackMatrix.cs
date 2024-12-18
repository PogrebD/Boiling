namespace SharpMath.Matrices;

public readonly ref struct StackMatrix
{
    private readonly Span<double> _values;
    public int Size { get; }

    public double this[int row, int column]
    {
        get => _values[GetSpanIndex(row, column)];

        set => _values[GetSpanIndex(row, column)] = value;
    }

    public StackMatrix(Span<double> values, int size)
    {
        if (values.Length != size * size)
            throw new ArgumentException();

        _values = values;
        Size = size;
    }

    private int GetSpanIndex(int row, int column)
    {
        return row * Size + column;
    }
}