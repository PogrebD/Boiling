using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.Splines;

public interface ISplineCreator<TPoint, TElement> : IAllocationRequired<Grid<TPoint, TElement>>
{
    public ISpline<TPoint> CreateSpline(FuncValue[] funcValues, double alpha);
}

public readonly record struct FuncValue(Point Point, double Value);