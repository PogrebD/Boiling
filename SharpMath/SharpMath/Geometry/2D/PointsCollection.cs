namespace SharpMath.Geometry._2D;

public class PointsCollection : IPointsCollection<Point>
{
    public int TotalPoints => _xAxis.Length * _yAxis.Length;
    public int XLength => _xAxis.Length;
    public int YLength => _yAxis.Length;
    public int ZLength => 0;

    public Point this[int index]
    {
        get
        {
            var row = index / _xAxis.Length;
            var column = index % _xAxis.Length;
            var x = _xAxis[column];
            var y = _yAxis[row];

            return new Point(x, y);
        }
    }
    
    private readonly double[] _xAxis;
    private readonly double[] _yAxis;

    public PointsCollection(double[] xAxis, double[] yAxis)
    {
        _xAxis = xAxis;
        _yAxis = yAxis;
    }
}