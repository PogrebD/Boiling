using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.BasisFunctions;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;

namespace SharpMath.Splines;

public class SplineLocalAssembler : ISplineStackLocalAssembler<Element, Point>
{
    private readonly IBasisFunctionsProvider<Element, Point> _basisFunctionsProvider;
    private IBasisFunction<Point>[] _basisFunctions;

    public SplineLocalAssembler(IBasisFunctionsProvider<Element, Point> basisFunctionsProvider)
    {
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public void AssembleBasisFunctions(Element element)
    {
        _basisFunctions = _basisFunctionsProvider.GetFunctions(element);
    }

    public void AssembleMatrix(Element element, Point point, double weight, StackMatrix matrix, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length * 4; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = weight * _basisFunctions[i].Evaluate(point) * _basisFunctions[j].Evaluate(point);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(Element element, FuncValue functionValue, double weight, Span<double> vector, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length * 4; i++)
        {
            vector[i] = weight * _basisFunctions[i].Evaluate(functionValue.Point) * functionValue.Value;
        }

        FillIndexes(element, indexes);
    }

    private static void FillIndexes(Element element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                indexes.Permutation[i * 4 + j] = element.NodeIndexes[i] * 4 + j;
            }
        }
    }
}