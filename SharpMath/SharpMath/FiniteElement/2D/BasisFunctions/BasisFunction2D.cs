using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement._2D.BasisFunctions;

public class BasisFunction2D : IBasisFunction<Point>
{
    private readonly Func<double, double> _xFunction;
    private readonly Func<double, double> _yFunction;

    public BasisFunction2D(Func<double, double> xFunction, Func<double, double> yFunction)
    {
        _xFunction = xFunction;
        _yFunction = yFunction;
    }

    public double Evaluate(Point point)
    {
        return Evaluate(point.X, point.Y);
    }

    public double Evaluate(double x, double y)
    {
        return _xFunction(x) * _yFunction(y);
    }
}