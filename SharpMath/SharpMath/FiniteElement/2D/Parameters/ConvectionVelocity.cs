using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Сylinder;

namespace SharpMath.FiniteElement._2D.Parameters;

public class ConvectionVelocity : IUniversalParameter<Point, Point>
{
    private readonly double _maxVelocity;
    private readonly double _height;
    private readonly double _radius;
    
    public ConvectionVelocity(IPointsCollection<Point> points, double maxVelocity)
    {
        var lb = points[0];
        var rt = points[points.XLength * points.YLength - 1];
        _height = rt.Z() - lb.Z();
        _radius = rt.R() - lb.R();
        
        _maxVelocity = maxVelocity;
    }
    
    public Point Get(Point point)
    {
        var lowerThanFirst = point.Z() <= _height * point.R() / _radius;
        var lowerThanSecond = point.Z() <= _height * (1 - point.R() / _radius);
        var heightQuarter = _height / 4;
        var radiusQuarter = _radius / 4;
        var factor = 1 - (lowerThanFirst, lowerThanSecond) switch
        {
            (true, true) => Math.Abs(point.Z() - heightQuarter) / heightQuarter,
            (true, false) => Math.Abs(point.R() - 3 * radiusQuarter) / radiusQuarter,
            (false, false) => Math.Abs(point.Z() - 3 * heightQuarter) / heightQuarter,
            (false, true) => Math.Abs(point.R() - radiusQuarter) / radiusQuarter,
        };
        Point direction = (lowerThanFirst, lowerThanSecond) switch
        {
            (true, true) => new (1, 0),
            (true, false) => new (0, 1),
            (false, false) => new (-1, 0),
            (false, true) => new (0, -1),
        };
        
        var velocity = direction * factor * _maxVelocity;

        return velocity;
    }
}