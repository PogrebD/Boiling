namespace SharpMath.Matrices.Sparse.Storages;

public readonly ref struct RefIndexValue
{
    public int ColumnIndex { get; }
    public double Value => _value[_valueIndex];

    public void SetValue(double value)
    {
        _value[_valueIndex] = value;
    }

    private readonly Span<double> _value;
    private readonly int _valueIndex;

    public RefIndexValue(int columnIndex, Span<double> value, int valueIndex)
    {
        _value = value;
        _valueIndex = valueIndex;
        ColumnIndex = columnIndex;
    }
}