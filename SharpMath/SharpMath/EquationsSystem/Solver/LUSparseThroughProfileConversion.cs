using SharpMath.Matrices.Converters;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Solver;

public class LUSparseThroughProfileConversion : ISLAESolver<SparseMatrix>
{
    public Vector Solve(Equation<SparseMatrix> equation)
    {
        var profile = MatrixConverter.Convert(equation.Matrix);
        var equationProfile = new Equation<ProfileMatrix>(profile, equation.Solution, equation.RightSide);
        var profileSolver = new LUProfile();
        
        return profileSolver.Solve(equationProfile);
    }
}