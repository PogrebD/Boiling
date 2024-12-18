namespace SharpMath.Matrices;

public class ImmutableMatrix : ArrayMatrix
{
    public override double this[int row, int column] => Values[row, column] * Coefficient;
    public double Coefficient { get; }

    public ImmutableMatrix(double[,] values)
        : this(values, 1d)
        { }

    public ImmutableMatrix(double[,] values, double coefficient)
        : base(values)
    {
        Coefficient = coefficient;
    }

    public ImmutableMatrix(ImmutableMatrix a, double coefficient)
        : base(a.Values)
    {
        Coefficient = coefficient;
    }

    public override ImmutableMatrix AsImmutable()
    {
        return this;
    }
    
    public override Matrix AsMutable()
    {
        return Clone();
    }
}