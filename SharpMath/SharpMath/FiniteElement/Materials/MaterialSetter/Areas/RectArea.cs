using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter.Areas;

public class RectArea : IMaterialArea<Point>
{
    private readonly Rectangle _area;

    public Rectangle Area => _area;
    public int MaterialId { get; }

    public RectArea(Rectangle area, int materialId)
    {
        _area = area;
        MaterialId = materialId;
    }
    
    public bool Contains(Point point)
    {
        return _area.Contains(point);
    }
}