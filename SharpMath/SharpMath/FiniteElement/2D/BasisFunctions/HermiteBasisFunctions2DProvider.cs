using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.FiniteElement.Materials;
using SharpMath.Geometry._2D;
using SharpMath.Matrices.Sparse;
using SharpMath.Splines;
using System.Reflection.Metadata.Ecma335;
using SharpMath.Matrices;

namespace SharpMath.FiniteElement._2D.BasisFunctions;

//Наверн надо абстрактный класс ещё сделать, потому что контексты разные могут быть
public class HermiteBasisFunctions2DProvider : IBasisFunctionsProvider<Element, Point>
{
    private readonly SplineContext<Point, Element, Matrix> _context;
    private static readonly IBasisFunction<Point>[] BasisFunctions2D;
    private static readonly Func<double, double>[] XBasisFunctions1D;
    private static readonly Func<double, double>[] YBasisFunctions1D;

    static HermiteBasisFunctions2DProvider()
    {
        BasisFunctions2D = new IBasisFunction<Point>[16];
        XBasisFunctions1D = new Func<double, double>[4];
        YBasisFunctions1D = new Func<double, double>[4];
    }

    public HermiteBasisFunctions2DProvider(SplineContext<Point, Element, Matrix> context)
    {
        _context = context;
    }

    public IBasisFunction<Point>[] GetFunctions(Element element)
    {
        var firstNodeOfElement = _context.Grid.Nodes[element.NodeIndexes[0]];
        var xBasisFunctions1D = BuildHermiteBasisFunctions1D(firstNodeOfElement.X, element.Width, XBasisFunctions1D);
        var yBasisFunctions1D = BuildHermiteBasisFunctions1D(firstNodeOfElement.Y, element.Length, YBasisFunctions1D);

        for (var i = 0; i < xBasisFunctions1D.Length; i++)
        {
            for (var j = 0; j < yBasisFunctions1D.Length; j++)
            {
                var basisFunctionIndex = i * 4 + j;
                BasisFunctions2D[basisFunctionIndex] = new BasisFunction2D(xBasisFunctions1D[Mu(basisFunctionIndex)], yBasisFunctions1D[Nu(basisFunctionIndex)]);
            }
        }

        return BasisFunctions2D;
    }

    //Потенциально надо сделать провайдер для одномерных и оттуда их создавать
    private static Func<double, double>[] BuildHermiteBasisFunctions1D(double leftCoordinateOfBound, double elementBoundSize, Func<double, double>[] basisFunctions1D)
    {
        basisFunctions1D[0] = coordinate => 1 - 3 * Math.Pow(Shift(coordinate), 2) + 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[1] = coordinate => elementBoundSize * (Shift(coordinate) - 2 * Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));
        basisFunctions1D[2] = coordinate => 3 * Math.Pow(Shift(coordinate), 2) - 2 * Math.Pow(Shift(coordinate), 3);
        basisFunctions1D[3] = coordinate => elementBoundSize * (-Math.Pow(Shift(coordinate), 2) + Math.Pow(Shift(coordinate), 3));

        return basisFunctions1D;

        double Shift(double coordinate) => (coordinate - leftCoordinateOfBound) / elementBoundSize;
    }

    private static int Mu(int i)
    {
        return 2 * (i / 4 % 2) + i % 2;
    }

    private static int Nu(int i)
    {
        return 2 * (i / 8) + i / 2 % 2;
    }
}