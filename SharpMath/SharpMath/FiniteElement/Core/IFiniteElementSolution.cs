namespace SharpMath.FiniteElement.Core;

public interface IFiniteElementSolution<in TPoint>
{
    public double Calculate(TPoint point);
    public double Derivative(TPoint point);
}