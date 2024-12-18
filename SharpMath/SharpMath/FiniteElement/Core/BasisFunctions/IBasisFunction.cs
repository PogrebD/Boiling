namespace SharpMath.FiniteElement.Core.BasisFunctions;

public interface IBasisFunction<in TPoint>
{
    public double Evaluate(TPoint point);
}