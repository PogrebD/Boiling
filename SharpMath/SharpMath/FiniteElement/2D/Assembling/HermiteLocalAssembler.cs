using SharpMath.FiniteElement._2D.Elements;
using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Splines;

namespace SharpMath.FiniteElement._2D.Assembling;

public class HermiteLocalAssembler : IStackLocalAssembler<Element>
{
    private readonly SplineContext<Point, Element, Matrix> _context;

    public HermiteLocalAssembler(SplineContext<Point, Element, Matrix> context)
    {
        _context = context;
    }

    public void AssembleMatrix(Element element, StackMatrix matrix, StackIndexPermutation indexes)
    {
        var stiffnessMatrixX = HermiteTemplateMatrices.Stiffness1D(element.Width);
        var stiffnessMatrixY = HermiteTemplateMatrices.Stiffness1D(element.Length);

        var massMatrixX = HermiteTemplateMatrices.Mass1D(element.Width);
        var massMatrixY = HermiteTemplateMatrices.Mass1D(element.Length);
        
        for (var i = 0; i < element.NodeIndexes.Length * 4; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                matrix[i, j] = _context.Alpha * 
                               (stiffnessMatrixX[Mu(i), Mu(j)] * massMatrixY[Nu(i), Nu(j)] + 
                               massMatrixX[Mu(i), Mu(j)] * stiffnessMatrixY[Nu(i), Nu(j)]);
                matrix[j, i] = matrix[i, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(Element element, Span<double> vector, StackIndexPermutation indexes)
    {
        throw new NotImplementedException();
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

    private static int Mu(int i)
    {
        return i % 4;
    }

    private static int Nu(int i)
    {
        return i / 4;
    }
}