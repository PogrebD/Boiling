using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.Geometry._2D;
using SharpMath.Matrices.Sparse;
using SharpMath.Matrices;

namespace SharpMath.Splines;

public class SplineEquationAssembler
{
    public Equation<Matrix> FinalEquation => _context.Equation;

    private readonly SplineContext<Point, Element, Matrix> _context;
    private readonly ISplineStackLocalAssembler<Element, Point> _splineLocalAssembler;
    private readonly IStackLocalAssembler<Element> _localAssembler;
    private readonly IStackInserter<Matrix> _inserter;

    public SplineEquationAssembler(
        SplineContext<Point, Element, Matrix> context,
        ISplineStackLocalAssembler<Element, Point> splineLocalAssembler,
        IStackLocalAssembler<Element> localAssembler,
        IStackInserter<Matrix> inserter
    )
    {
        _context = context;
        _splineLocalAssembler = splineLocalAssembler;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public SplineEquationAssembler BuildEquation(SplineContext<Point, Element, Matrix> context)
    {
        var equation = context.Equation;

        var matrix = new StackMatrix(stackalloc double[16 * 16], 16);
        Span<double> vector = stackalloc double[16];
        var indexes = new StackIndexPermutation(stackalloc int[16]);

        for (var i = 0; i < context.FunctionValues.Length; i++)
        {
            var currentFunctionValue = context.FunctionValues[i];
            var currentWeight = context.Weights[i];
            var element = context.Grid.Elements.First(e => ElementHas(e, currentFunctionValue.Point));

            _splineLocalAssembler.AssembleBasisFunctions(element);
            _splineLocalAssembler.AssembleMatrix(element, currentFunctionValue.Point, currentWeight, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);

            _splineLocalAssembler.AssembleRightSide(element, currentFunctionValue, currentWeight,  vector, indexes);
            var localRightSide = new StackLocalVector(vector, indexes);
            _inserter.InsertVector(equation.RightSide, localRightSide);
        }

        foreach (var element in context.Grid.Elements)
        {
            _localAssembler.AssembleMatrix(element, matrix, indexes);
            var localMatrix = new StackLocalMatrix(matrix, indexes);
            _inserter.InsertMatrix(equation.Matrix, localMatrix);
        }

        return this;
    }

    private bool ElementHas(Element element, Point node)
    {
        var leftBottom = _context.Grid.Nodes[element.NodeIndexes[0]];
        var rightTop = _context.Grid.Nodes[element.NodeIndexes[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}