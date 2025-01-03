﻿using Boiling.FiniteElement;
using Boiling.FiniteElement._2D;
using Boiling.FiniteElement._2D.Assembling;
using Boiling.FiniteElement.Time;
using SharpMath;
using SharpMath.Computations.Integration;
using SharpMath.EquationsSystem.Solver;
using SharpMath.FiniteElement;
using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Assembling;
using SharpMath.FiniteElement._2D.BasisFunctions;
using SharpMath.FiniteElement._2D.Parameters;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Third;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.FiniteElement.Materials.Boiling;
using SharpMath.FiniteElement.Providers.Density;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Geometry._2D.Сylinder;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;
using System.Linq;
using SharpMath.FiniteElement.Materials.MaterialSetter.Areas;
using SharpMath.Geometry.Splitting;

namespace Boiling.DirectSolver;

public class BoilingDirectSolver : IAllocationRequired<Grid<Point, Element>>, IAllocationRequired<IMaterialProvider<BoilingMaterial>>, IAllocationRequired<double[]>
{
    private Vector PreviousSolution => _context.TimeSolutions[_currentTimeLayer - 1];
    private double CurrentTime => _context.TimeLayers[_currentTimeLayer];
    private double PreviousTime => _context.TimeLayers[_currentTimeLayer - 1];

    private readonly ISLAESolver<SparseMatrix> _slaeSolver;
    private IMaterialProvider<BoilingMaterial> _materials;
    private BoilingContext<Point, Element, SparseMatrix> _context = null!;
    private BoilingEquationAssembler _assembler = null!;

    private int _currentTimeLayer;

    public BoilingDirectSolver(ISLAESolver<SparseMatrix> slaeSolver)
    {
        _slaeSolver = slaeSolver;
    }

    public void Allocate(Grid<Point, Element> param)
    {
        _context = CreateContext(param);
    }

    public void Allocate(IMaterialProvider<BoilingMaterial> param)
    {
        _materials = param;
        _assembler = CreateAssembler(_context);
    }

    public void Allocate(double[] param)
    {
        _context.TimeLayers = param;
        _context.TimeSolutions = new Vector[param.Length];
    }

    public TimeFiniteElementSolution2D Solve(Vector initialSolution)
    {
        _context.TimeSolutions[0] = initialSolution;

        for (_currentTimeLayer = 1; _currentTimeLayer < _context.TimeLayers.Length; _currentTimeLayer++)
        {
            //_context.FirstConditions = CreateFirst(_context.Grid);
            _assembler
                .BuildEquation(PreviousSolution, CurrentTime, PreviousTime)
                .ApplySecondBoundary(_context);

            _slaeSolver.Solve(_assembler.CurrentTimeLayerEquation);

            _context.TimeSolutions[_currentTimeLayer] = _assembler.CurrentTimeLayerEquation.Solution; //_assembler.CurrentTimeLayerEquation.Solution
            if (_currentTimeLayer > 1) 
                _context.TimeSolutions[_currentTimeLayer - 2] = null;

            int[] TimeSloy = [1, 10, 50, 100, 250, 500, 1000];
            
            const double r = 0.07; //радиус
            const double h = 0.08; //высота(z)
            var water = new RectArea(
                new Rectangle(
                    0, 0,
                    r, h
                ),
                materialId: 0
            );
            var areas = new AreasMaterialSetterFactory(
                [water],
                defaultMaterialIdId: 0
            );
            
            var outputGrid = new GridBuilder()
                .SetXAxis(new AxisSplitParameter(
                    [0, r],
                    new UniformSplitter(70)
                ))
                .SetYAxis(new AxisSplitParameter(
                    [0, h],
                    new UniformSplitter(80)
                ))
                .SetMaterialSetterFactory(areas)
                .Build();

            if (TimeSloy.Contains((int)CurrentTime))
            {
                var filePath = $@"C:\Users\dimap\RiderProjects\BoilingTrue\View\View\velocity0,01\data{(int)CurrentTime}.txt";
                using (var writer = new StreamWriter(filePath))
                { 
                    for (var i = 0; i < outputGrid.Nodes.TotalPoints; i++)
                    {
                        writer.WriteLine($"{ outputGrid.Nodes[i].X:F5} { outputGrid.Nodes[i].Y:F5} {_assembler.CurrentTimeLayerEquation.Solution[i]:E5}");
                    }
                }
            }
            
            
            Console.Write($"{_currentTimeLayer}\r");
        }

        return new TimeFiniteElementSolution2D(new BilinearBasisFunctionsProvider(_context), _context.Grid, _context.TimeSolutions, _context.TimeLayers);
    }

    private BoilingContext<Point, Element, SparseMatrix> CreateContext(Grid<Point, Element> grid)
    {
        var portraitBuilder = new SparseMatrixPortraitBuilder();
        var stiffnessAndVelocityMatrix = portraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints);
        var massMatrix = portraitBuilder.Build(grid.Elements, grid.Nodes.TotalPoints);

        var context = new BoilingContext<Point, Element, SparseMatrix>
        {
            Grid = grid,
            Equation = null,
            Materials = null,
            DensityFunctionProvider = null,
            FirstConditions = null,
            SecondConditions = CreateSecond(grid),
            ThirdConditions = CreateThird(grid),
            StiffnessAndVelocityMatrix = stiffnessAndVelocityMatrix,
            MassMatrix = massMatrix,
            RightPart = Vector.Create(grid.Nodes.TotalPoints),
            TimeLayers = null,
            TimeSolutions = null,
        };

        return context;
    }

    private FirstCondition[] CreateFirst(Grid<Point, Element> grid)
    {
        var u = new Func<Point, double, double>((p, t) => p.R() + t);

        return EnumerateBottomElementsIndexes(grid)
            .SelectMany(elementIndex =>
            {
                var element = grid.Elements[elementIndex];
                var nodeIndexes = element.GetBoundNodeIndexes(Bound.Bottom);
                var conditionOne = new FirstCondition(nodeIndexes[0], u(grid.Nodes[nodeIndexes[0]], CurrentTime));
                var conditionTwo = new FirstCondition(nodeIndexes[1], u(grid.Nodes[nodeIndexes[1]], CurrentTime));

                return new[] { conditionOne, conditionTwo };
            })
            .Concat(EnumerateRightElementsIndexes(grid)
                .SelectMany(elementIndex =>
                {
                    var element = grid.Elements[elementIndex];
                    var nodeIndexes = element.GetBoundNodeIndexes(Bound.Right);
                    var conditionOne = new FirstCondition(nodeIndexes[0], u(grid.Nodes[nodeIndexes[0]], CurrentTime));
                    var conditionTwo = new FirstCondition(nodeIndexes[1], u(grid.Nodes[nodeIndexes[1]], CurrentTime));

                    return new[] { conditionOne, conditionTwo };
                })
            )
            .Concat(EnumerateTopElementIndexes(grid)
                .SelectMany(elementIndex =>
                {
                    var element = grid.Elements[elementIndex];
                    var nodeIndexes = element.GetBoundNodeIndexes(Bound.Top);
                    var conditionOne = new FirstCondition(nodeIndexes[0], u(grid.Nodes[nodeIndexes[0]], CurrentTime));
                    var conditionTwo = new FirstCondition(nodeIndexes[1], u(grid.Nodes[nodeIndexes[1]], CurrentTime));

                    return new[] { conditionOne, conditionTwo };
                })
            )
            .Concat(EnumerateLeftElementsIndexes(grid)
                .SelectMany(elementIndex =>
                {
                    var element = grid.Elements[elementIndex];
                    var nodeIndexes = element.GetBoundNodeIndexes(Bound.Left);
                    var conditionOne = new FirstCondition(nodeIndexes[0], u(grid.Nodes[nodeIndexes[0]], CurrentTime));
                    var conditionTwo = new FirstCondition(nodeIndexes[1], u(grid.Nodes[nodeIndexes[1]], CurrentTime));

                    return new[] { conditionOne, conditionTwo };
                })
            )
            .ToArray();
    }

    private SecondCondition[] CreateSecond(Grid<Point, Element> grid)
    {
        var panRadius = grid.Nodes[grid.Nodes.TotalPoints - 1].R();
        var theta = 1000d / (Math.PI * panRadius * panRadius);
        return EnumerateBottomElementsIndexes(grid)
            .Select(elementIndex => new SecondCondition(elementIndex, Bound.Bottom, [theta, theta], ComponentType.Real))
            .ToArray();
    }

    private ThirdCondition[] CreateThird(Grid<Point, Element> grid)
    {
        return EnumerateRightElementsIndexes(grid)
            .Select(elementIndex => new ThirdCondition(elementIndex, Bound.Right, [24d, 24d], 200d))
            .Concat(EnumerateTopElementIndexes(grid)
                .Select(elementIndex => new ThirdCondition(elementIndex, Bound.Top, [24d, 24d], 200d)))
            .ToArray();
    }

    private static IEnumerable<int> EnumerateBottomElementsIndexes(Grid<Point, Element> grid)
    {
        var xAxisElements = grid.Nodes.XLength - 1;

        for (var i = 0; i < xAxisElements; i++)
        {
            yield return i;
        }
    }

    private static IEnumerable<int> EnumerateLeftElementsIndexes(Grid<Point, Element> grid)
    {
        var xAxisElements = grid.Nodes.XLength - 1;
        var yAxisElements = grid.Nodes.YLength - 1;

        for (var i = 0; i < yAxisElements; i++)
        {
            yield return i * xAxisElements;
        }
    }

    private static IEnumerable<int> EnumerateRightElementsIndexes(Grid<Point, Element> grid)
    {
        var yAxisElements = grid.Nodes.YLength - 1;

        for (var i = 0; i < yAxisElements; i++)
        {
            yield return (i + 1) * yAxisElements - 1;
        }
    }

    private static IEnumerable<int> EnumerateTopElementIndexes(Grid<Point, Element> grid)
    {
        var xAxisElements = grid.Nodes.XLength - 1;
        var yAxisElements = grid.Nodes.YLength - 1;

        for (var i = xAxisElements * (yAxisElements - 1); i <= xAxisElements * yAxisElements - 1; i++)
        {
            yield return i;
        }
    }

    private BoilingEquationAssembler CreateAssembler(BoilingContext<Point, Element, SparseMatrix> context)
    {
        var inserter = new Inserter();

        return new BoilingEquationAssembler(
            context,
            new StiffnessMatrixLocalAssembler(context, _materials),
            new MassMatrixLocalAssembler(context, _materials),
            new VelocityMatrixLocalAssembler(
                context,
                _materials,
                new ConvectionVelocity(context.Grid.Nodes, 0.001),
                new DoubleIntegration(GaussMethodConfig.UseGaussMethodTwo(1)),
                new BilinearBasisFunctionsProvider(context)
            ),
            new RightPartAssembler(context),
            inserter,
            new GaussExcluderSparse(),
            new CylinderSecondBoundaryApplier(context, inserter),
            new CylinderThirdBoundaryApplier(context, inserter)
        );
    }
}