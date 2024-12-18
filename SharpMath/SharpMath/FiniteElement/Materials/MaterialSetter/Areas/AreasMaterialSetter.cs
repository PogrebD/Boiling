using SharpMath.Geometry;
using SharpMath.Geometry._2D;
// ReSharper disable PossibleMultipleEnumeration

namespace SharpMath.FiniteElement.Materials.MaterialSetter.Areas;

public class AreasMaterialSetter : IMaterialSetter
{
    private readonly IPointsCollection<Point> _points;
    private readonly IMaterialArea<Point>[] _areas;
    private readonly int _defaultMaterialId;

    public AreasMaterialSetter(IPointsCollection<Point> points, IMaterialArea<Point>[] areas, int defaultMaterialIdId = 0)
    {
        _points = points;
        _areas = areas;
        _defaultMaterialId = defaultMaterialIdId;
    }

    public void SetMaterial(IFiniteElement element)
    {
        var points = element.NodeIndexes.Select(i => _points[i]);

        foreach (var area in _areas)
        {
            if (!points.All(area.Contains))
            {
                continue;
            }

            element.MaterialId = area.MaterialId;
            return;
        }

        element.MaterialId = _defaultMaterialId;
    }
}