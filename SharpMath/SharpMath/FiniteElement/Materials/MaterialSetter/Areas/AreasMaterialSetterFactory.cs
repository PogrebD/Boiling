using SharpMath.FiniteElement.Materials.MaterialSetter.Factory;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
namespace SharpMath.FiniteElement.Materials.MaterialSetter.Areas;

public class AreasMaterialSetterFactory : IMaterialSetterFactory
{
    private readonly IMaterialArea<Point>[] _sections;
    private readonly int _defaultMaterialId;

    public AreasMaterialSetterFactory(IMaterialArea<Point>[] sections, int defaultMaterialIdId = 0)
    {

        _sections = sections;
        _defaultMaterialId = defaultMaterialIdId;
    }

    public IMaterialSetter Create(IPointsCollection<Point> points)
    {
        return new AreasMaterialSetter(points, _sections, _defaultMaterialId);
    }
}