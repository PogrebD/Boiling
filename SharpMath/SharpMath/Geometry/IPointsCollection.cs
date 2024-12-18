namespace SharpMath.Geometry;

public interface IPointsCollection<out TPoint>
{
    public int TotalPoints { get; }
    public TPoint this[int index] { get; }
    public int XLength { get; }
    public int YLength { get; }
    public int ZLength { get; }
}