using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Сylinder;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Third;

public class CylinderThirdBoundaryApplier : IThirdBoundaryApplier<SparseMatrix>
{
    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly IStackInserter<SparseMatrix> _inserter;

    public CylinderThirdBoundaryApplier(Context<Point, Element, SparseMatrix> context, IStackInserter<SparseMatrix> inserter)
    {
        _context = context;
        _inserter = inserter;
    }

    public void Apply(Equation<SparseMatrix> equation, ThirdCondition condition)
    {
        var element = _context.Grid.Elements[condition.ElementId];

        var indexes = element.GetBoundNodeIndexes(condition.Bound, stackalloc int[2]);

        var massMatrix1D = condition.Bound is Bound.Left or Bound.Right ? 
            CylinderTemplateMatrices.MassZ1D(element.Length) : 
            CylinderTemplateMatrices.MassR1D(_context.Grid.Nodes[element.NodeIndexes[0]].R(), element.Width);

        var conditionImpactMatrix = LinAl.Multiply(condition.Beta, massMatrix1D, new StackMatrix(stackalloc double[2 * 2], 2));
        var localMatrix = new StackLocalMatrix(conditionImpactMatrix, new StackIndexPermutation(indexes));

        var conditionImpactVector = LinAl.Multiply(conditionImpactMatrix, condition.UBeta, stackalloc double[2]);
        var localVector = new StackLocalVector(conditionImpactVector, new StackIndexPermutation(indexes));

        _inserter.InsertMatrix(equation.Matrix, localMatrix);
        _inserter.InsertVector(equation.RightSide, localVector);
    }
}