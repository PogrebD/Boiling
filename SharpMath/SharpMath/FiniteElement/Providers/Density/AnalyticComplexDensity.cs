using System.Numerics;
using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.Geometry._2D;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement.Providers.Density;

public class AnalyticComplexDensity : INodeDefinedParameter<Complex>
{
    private readonly Context<Point, Element, SparseMatrix> _context;
    private readonly Func<Point, Complex> _func;

    public AnalyticComplexDensity(Context<Point, Element, SparseMatrix> context, Func<Point, Complex> func)
    {
        _context = context;
        _func = func;
    }

    public Complex Get(int nodeIndex)
    {
        var node = _context.Grid.Nodes[nodeIndex];
        return _func(node);
    }
}