using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.EquationsSystem.Preconditions;

public interface IPreconditionerFactory
{
    public IPreconditioner CreatePreconditioner(SymmetricSparseMatrix matrix);
}