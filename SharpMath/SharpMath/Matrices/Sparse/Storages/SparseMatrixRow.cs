namespace SharpMath.Matrices.Sparse.Storages;

public readonly ref struct SparseMatrixRow
{
    public int Index { get; }

    public ref double this[int column]
    {
        get
        {
            if (!HasColumn(column, out var valueIndex))
                throw new IndexOutOfRangeException();

            return ref _values[valueIndex];
        }
    }

    public bool HasColumn(int column)
    {
        var valueIndex = _columnIndexes.BinarySearch(column);
        return valueIndex >= 0;
    }

    private bool HasColumn(int column, out int columnIndex)
    {
        columnIndex = _columnIndexes.BinarySearch(column);
        return columnIndex >= 0;
    }

    private readonly ReadOnlySpan<int> _columnIndexes;
    private readonly Span<double> _values;

    public SparseMatrixRow(ReadOnlySpan<int> columnIndexes, Span<double> values, int index)
    {
        Index = index;
        _columnIndexes = columnIndexes;
        _values = values;
    }

    public Enumerator GetEnumerator() => new(_columnIndexes, _values);

    public ref struct Enumerator
    {
        private readonly ReadOnlySpan<int> _columnIndexes;
        private readonly Span<double> _values;

        private int _index;

        internal Enumerator(ReadOnlySpan<int> columnIndexes, Span<double> values)
        {
            _columnIndexes = columnIndexes;
            _values = values;
            _index = -1;
        }

        public bool MoveNext()
        {
            int index = _index + 1;
            if (index >= _values.Length) return false;
            _index = index;
            return true;

        }

        public RefIndexValue Current => new(_columnIndexes[_index], _values, _index);
    }
}