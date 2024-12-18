using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Preconditions;


public interface IPreconditioner
{
    public Vector MultiplyOn(Vector v, Vector? resultMemory = null);
}

public interface IPreconditioner<out TResult>
{
    public TResult Decompose(SparseMatrix matrix);
}