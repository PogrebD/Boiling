using SharpMath.Geometry;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Materials.MaterialSetter.Factory;

public class NullMaterialSetterFactory : IMaterialSetterFactory
{
    public IMaterialSetter Create(IPointsCollection<Point> points)
    {
        return new NullMaterialSetter();
    }
}