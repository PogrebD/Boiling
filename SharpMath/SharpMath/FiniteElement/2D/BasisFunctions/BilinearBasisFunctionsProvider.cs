using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry._2D;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement._2D.BasisFunctions;

public class BilinearBasisFunctionsProvider : IBasisFunctionsProvider<Element, Point>
{
    private readonly Context<Point, Element, SparseMatrix> _context;
    private static readonly IBasisFunction<Point>[] BasisFunctions2D;
    private static readonly Func<double, double>[] XBasisFunctions1D;
    private static readonly Func<double, double>[] YBasisFunctions1D;

    static BilinearBasisFunctionsProvider()
    {
	    BasisFunctions2D = new IBasisFunction<Point>[4];
	    XBasisFunctions1D = new Func<double, double>[2];
	    YBasisFunctions1D = new Func<double, double>[2];
    }

    public BilinearBasisFunctionsProvider(Context<Point, Element, SparseMatrix> context)
    {
	    _context = context;
    }

    public IBasisFunction<Point>[] GetFunctions(Element element)
    {
	    XBasisFunctions1D[0] = BuildFirstFunction(_context.Grid.Nodes[element.NodeIndexes[1]].X, element.Width);
	    XBasisFunctions1D[1] = BuildSecondFunction(_context.Grid.Nodes[element.NodeIndexes[0]].X, element.Width);

	    YBasisFunctions1D[0] = BuildFirstFunction(_context.Grid.Nodes[element.NodeIndexes[2]].Y, element.Length);
	    YBasisFunctions1D[1] = BuildSecondFunction(_context.Grid.Nodes[element.NodeIndexes[0]].Y, element.Length);

	    for (var i = 0; i < YBasisFunctions1D.Length; i++)
	    {
		    for (var j = 0; j < XBasisFunctions1D.Length; j++)
		    {
			    BasisFunctions2D[i * YBasisFunctions1D.Length + j] = new BasisFunction2D(XBasisFunctions1D[j], YBasisFunctions1D[i]);
		    }
	    }

	    return BasisFunctions2D;
    }

    private static Func<double, double> BuildFirstFunction(double rightCoordinate, double elementBoundSize)
    {
        return coordinate => (rightCoordinate - coordinate) / elementBoundSize;
    }

    private static Func<double, double> BuildSecondFunction(double leftCoordinate, double elementBoundSize)
    {
	    return coordinate => (coordinate - leftCoordinate) / elementBoundSize;
    }
}