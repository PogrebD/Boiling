using SharpMath.EquationsSystem.Solver;
using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Assembling;
using SharpMath.FiniteElement._2D.BasisFunctions;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.FiniteElement.Providers.Density;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.Splines;

public class SmoothingSplineCreator : ISplineCreator<Point, Element>
{
    private HermiteBasisFunctions2DProvider _basisFunctionsProvider;
    private SplineContext<Point, Element, Matrix> _context;
    private readonly GaussZeidelSolver _slaeSolver;
    private SplineEquationAssembler _equationAssembler;
    private bool _allocated;

    public SmoothingSplineCreator(GaussZeidelSolver slaeSolver)
    {
        _slaeSolver = slaeSolver;
    }

    public void Allocate(Grid<Point, Element> grid)
    {
        if (_allocated)
        {
            return;
        }
        _context = CreateContext(grid);
        _equationAssembler = CreateAssembler(_context);
        _slaeSolver.Allocate(grid.Nodes.TotalPoints * 4);
        _allocated = true;
    }

    public ISpline<Point> CreateSpline(FuncValue[] funcValues, double alpha)
    {
        _context.FunctionValues = funcValues;
        _context.Weights = CalculateWeights(funcValues);
        _context.Alpha = alpha;

        _equationAssembler.BuildEquation(_context);

        var solution = _slaeSolver.Solve(_context.Equation.Matrix, _context.Equation.RightSide);

        for (var i = 0; i < _context.Equation.Solution.Length; i++)
        {
            _context.Equation.Solution[i] = solution[i];
        }

        return new SmoothingSpline(_basisFunctionsProvider, _context.Grid, _context.Equation.Solution);
    }

    private SplineContext<Point, Element, Matrix> CreateContext(Grid<Point, Element> grid)
    {
        var context = new SplineContext<Point, Element, Matrix>
        {
            Grid = grid,
            Equation = new Equation<Matrix>(
                Matrix: new Matrix(new double[grid.Nodes.TotalPoints * 4, grid.Nodes.TotalPoints * 4]),
                RightSide: Vector.Create(grid.Nodes.TotalPoints * 4),
                Solution: Vector.Create(grid.Nodes.TotalPoints * 4)
            ),
            FunctionValues = null,
            Weights = null,
            Alpha = 0,
            Beta = 0
        };

        return context;
    }

    private SplineEquationAssembler CreateAssembler(SplineContext<Point, Element, Matrix> context)
    {
        _basisFunctionsProvider = new HermiteBasisFunctions2DProvider(context);
        return new SplineEquationAssembler(
            context,
            new SplineLocalAssembler(_basisFunctionsProvider),
            new HermiteLocalAssembler(context),
            new DenseMatrixInserter()
        );
    }

    private static double[] CalculateWeights(FuncValue[] funcValues)
    {
        var weights = new double[funcValues.Length];

        for (var i = 0; i < funcValues.Length; i++)
        {
            weights[i] = 1;
        }

        return weights;
    }
}