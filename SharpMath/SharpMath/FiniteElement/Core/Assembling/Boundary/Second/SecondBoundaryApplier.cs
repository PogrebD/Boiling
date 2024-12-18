using SharpMath.FiniteElement._2D;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.FiniteElement.Core.Harmonic;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

public class SecondBoundaryApplier : ISecondBoundaryApplier<SparseMatrix>
{
    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly IStackInserter<SparseMatrix> _inserter;

    public SecondBoundaryApplier(Context<Point, Element, SparseMatrix> context, IStackInserter<SparseMatrix> inserter)
    {
        _context = context;
        _inserter = inserter;
    }

    public void Apply(Equation<SparseMatrix> equation, SecondCondition condition)
    {
        var element = _context.Grid.Elements[condition.ElementId];

        var indexes = element.GetBoundNodeIndexes(condition.Bound, stackalloc int[2]);
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] *= 2;
            if (condition.Type is ComponentType.Imaginary)
                indexes[i] += 1;
        }

        var defaultMass = new StackMatrix(stackalloc double[]
        {
            2, 1,
            1, 2
        }, 2);

        var massCoef = condition.Bound switch
        {
            Bound.Bottom or Bound.Top => element.Length / 6d,
            Bound.Left or Bound.Right => element.Width / 6d,
            _ => throw new ArgumentOutOfRangeException()
        };
        LinAl.Multiply(massCoef, defaultMass, defaultMass);

        var conditionImpact = LinAl.Multiply(defaultMass, condition.Thetta, stackalloc double[2]);
        var local = new StackLocalVector(conditionImpact, new StackIndexPermutation(indexes));
        _inserter.InsertVector(equation.RightSide, local);
    }
}