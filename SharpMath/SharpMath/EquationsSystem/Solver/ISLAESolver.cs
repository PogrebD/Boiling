using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Solver;

public interface ISLAESolver<TMatrix>
{
    public Vector Solve(Equation<TMatrix> equation);
}
