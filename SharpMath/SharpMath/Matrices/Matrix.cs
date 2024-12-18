namespace SharpMath.Matrices;

public class Matrix : ArrayMatrix
{
    public new double this[int row, int column]
    {
        set => Values[row, column] = value; 
        get => base[row, column];
    }

    public override ImmutableMatrix AsImmutable()
    {
        return new ImmutableMatrix(Values);
    }

    public override Matrix AsMutable()
    {
        return this;
    }

    public Matrix(double[,] values) 
        : base(values) 
    { }
}