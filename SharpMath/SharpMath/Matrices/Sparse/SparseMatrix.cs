namespace SharpMath.Matrices.Sparse;

public class SparseMatrix
{
    public double[] Diagonal { get; set; }
    public double[] LowerValues { get; set; }
    public double[] UpperValues { get; set; }
    public int[] RowsIndexes { get; }
    public int[] ColumnsIndexes { get; }

    public int RowsCount => Diagonal.Length;
    public int ColumnsCount => Diagonal.Length;
    public int this[int rowIndex, int columnIndex] =>
        Array.IndexOf(ColumnsIndexes, columnIndex, RowsIndexes[rowIndex],
            RowsIndexes[rowIndex + 1] - RowsIndexes[rowIndex]);

    public SparseMatrix(int[] rowsIndexes, int[] columnsIndexes)
    {
        Diagonal = new double[rowsIndexes.Length - 1];
        LowerValues = new double[rowsIndexes[^1]];
        UpperValues = new double[rowsIndexes[^1]];
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
    }

    public SparseMatrix
    (
        int[] rowsIndexes,
        int[] columnsIndexes,
        double[] diagonal,
        double[] lowerValues,
        double[] upperValues
    )
    {
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
        Diagonal = diagonal;
        LowerValues = lowerValues;
        UpperValues = upperValues;
    }

    public SparseMatrix Clone()
    {
        var rowIndexes = new int[RowsIndexes.Length];
        var columnIndexes = new int[ColumnsIndexes.Length];
        var diagonal = new double[Diagonal.Length];
        var lowerValues = new double[LowerValues.Length];
        var upperValues = new double[UpperValues.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);
        Array.Copy(ColumnsIndexes, columnIndexes, ColumnsIndexes.Length);
        Array.Copy(Diagonal, diagonal, Diagonal.Length);
        Array.Copy(LowerValues, lowerValues, LowerValues.Length);
        Array.Copy(UpperValues, upperValues, UpperValues.Length);

        return new SparseMatrix(rowIndexes, columnIndexes, diagonal, lowerValues, upperValues);
    }

    public int[] CloneRows()
    {
        var rowIndexes = new int[RowsIndexes.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);

        return rowIndexes;
    }

    public double[] CloneDiagonal()
    {
        var diagonal = new double[Diagonal.Length];

        Array.Copy(Diagonal, diagonal, Diagonal.Length);

        return diagonal;
    }
}