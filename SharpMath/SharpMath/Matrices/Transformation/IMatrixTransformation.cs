using SharpMath.Vectors;

namespace SharpMath.Matrices.Transformation;

public interface IMatrixTransformation
{
    public Vector SolveEqualTo(Vector rightSide);
}