using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry;
using SharpMath.Geometry._2D;
using SharpMath.Vectors;
using System;

namespace SharpMath.Splines;

public class SmoothingSpline : ISpline<Point>
{
    private readonly IBasisFunctionsProvider<Element, Point> _basisFunctionsProvider;
    private readonly Grid<Point, Element> _grid;
    private readonly Vector _qValues;

    public SmoothingSpline(
        IBasisFunctionsProvider<Element, Point> basisFunctionsProvider,
        Grid<Point, Element> grid, 
        Vector qValues
    )
    {
        _basisFunctionsProvider = basisFunctionsProvider;
        _grid = grid;
        _qValues = qValues;
    }

    public double Calculate(Point point)
    {
        var element = _grid.Elements.First(e => ElementHas(e, point));

        var basisFunctions = _basisFunctionsProvider.GetFunctions(element);

        var sum = 0d;

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                sum += _qValues[element.NodeIndexes[i] * 4 + j] * basisFunctions[i * 4 + j].Evaluate(point);
            }
        }

        return sum;
    }

    private bool ElementHas(Element element, Point node)
    {
        var leftBottom = _grid.Nodes[element.NodeIndexes[0]];
        var rightTop = _grid.Nodes[element.NodeIndexes[^1]];

        return leftBottom.X <= node.X && node.X <= rightTop.X && 
               leftBottom.Y <= node.Y && node.Y <= rightTop.Y;
    }
}