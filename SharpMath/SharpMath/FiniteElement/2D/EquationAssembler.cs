using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

namespace SharpMath.FiniteElement._2D;

public class EquationAssembler
{
    public Equation<SparseMatrix> FinalEquation => _context.Equation;

    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly IStackLocalAssembler<Element> _localAssembler;
    private readonly IStackInserter<SparseMatrix> _inserter;
    private readonly IFirstBoundaryApplier<SparseMatrix> _firstBoundaryApplier;
    private readonly ISecondBoundaryApplier<SparseMatrix> _secondBoundaryApplier;

    public EquationAssembler(
        Context<Point, Element, SparseMatrix> context,
        IStackLocalAssembler<Element> localAssembler,
        IStackInserter<SparseMatrix> inserter,
        IFirstBoundaryApplier<SparseMatrix> firstBoundaryApplier,
        ISecondBoundaryApplier<SparseMatrix> secondBoundaryApplier
    )
    {
        _context = context;
        _localAssembler = localAssembler;
        _inserter = inserter;
        _firstBoundaryApplier = firstBoundaryApplier;
        _secondBoundaryApplier = secondBoundaryApplier;
    }

    public EquationAssembler BuildEquation(Context<Point, Element, SparseMatrix> context)
    {
        var equation = context.Equation;

        var matrix = new StackMatrix(stackalloc double[8 * 8], 8);
        Span<double> vector = stackalloc double[8];
        var indexes = new StackIndexPermutation(stackalloc int[8]);

        foreach (var element in context.Grid.Elements)
        {
            _localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);
            
            _localAssembler.AssembleRightSide(element, vector, indexes);
             var localRightSide = new StackLocalVector(vector, indexes);
            _inserter.InsertVector(equation.RightSide, localRightSide);
        }

        return this;
    }

    public EquationAssembler ApplyFirstBoundary(Context<Point, Element, SparseMatrix> context)
    {
        var equation = context.Equation;

        foreach (var condition in _context.FirstConditions)
        {
            _firstBoundaryApplier.Apply(equation, condition);
        }

        return this;
    }

    public EquationAssembler ApplySecondConditions(Context<Point, Element, SparseMatrix> context)
    {
        foreach (var condition in context.SecondConditions)
        {
            _secondBoundaryApplier.Apply(context.Equation, condition);
        }

        return this;
    }
}