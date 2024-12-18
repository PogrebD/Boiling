namespace SharpMath.Matrices;

public abstract class ArrayMatrix : MatrixBase
{
    public override double this[int row, int column] => Values[row, column];
    public override int Rows => Values.GetLength(0);
    public override int Columns => Values.GetLength(1);

    protected readonly double[,] Values;

    protected ArrayMatrix(double[,] values)
    {
        Values = values;
    }
}