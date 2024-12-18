using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.EquationsSystem.Preconditions.Diagonal;

public class DiagonalPreconditionerFactory : IPreconditionerFactory
{
    public IPreconditioner CreatePreconditioner(SymmetricSparseMatrix matrix)
    {
        return new DiagonalPreconditioner(matrix.Diagonal);
    }
}