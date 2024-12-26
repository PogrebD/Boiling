using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Assembling;
using SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Сylinder;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

public class CylinderSecondBoundaryApplier : ISecondBoundaryApplier<SparseMatrix>
{
    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly IStackInserter<SparseMatrix> _inserter;

    public CylinderSecondBoundaryApplier(Context<Point, Element, SparseMatrix> context, IStackInserter<SparseMatrix> inserter)
    {
        _context = context;
        _inserter = inserter;
    }

    public void Apply(Equation<SparseMatrix> equation, SecondCondition condition)
    {
        var element = _context.Grid.Elements[condition.ElementId];

        var indexes = element.GetBoundNodeIndexes(condition.Bound, stackalloc int[2]);

        var massMatrix1D = condition.Bound is Bound.Left or Bound.Right ? 
             CylinderTemplateMatrices.MassZ1D(element.Width) : 
             CylinderTemplateMatrices.MassR1D(_context.Grid.Nodes[element.NodeIndexes[0]].R(), element.Length);

        var conditionImpact = LinAl.Multiply(massMatrix1D, condition.Thetta, stackalloc double[2]);
        var localVector = new StackLocalVector(conditionImpact, new StackIndexPermutation(indexes));

        _inserter.InsertVector(equation.RightSide, localVector);
    }
}