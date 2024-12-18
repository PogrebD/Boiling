namespace SharpMath.Geometry;

public class Grid<TPoint, TElement>
{
    public IPointsCollection<TPoint> Nodes { get; }
    public TElement[] Elements { get; }

    public Grid(
        IPointsCollection<TPoint> nodes,
        IEnumerable<TElement> elements
    )
    {
        Nodes = nodes;
        Elements = elements.ToArray();
    }
}