using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement._2D.Assembling;

public class HarmonicLocalAssembler : IStackLocalAssembler<Element>
{
    public static readonly ImmutableMatrix MassTemplate;
    public static readonly ImmutableMatrix StiffnessTemplate1;
    public static readonly ImmutableMatrix StiffnessTemplate2;

    static HarmonicLocalAssembler()
    {
        StiffnessTemplate1 = new ImmutableMatrix(new double[,]
        {
            {2, -2, 1, -1},
            {-2, 2, -1, 1},
            {1, -1, 2, -2},
            {-1, 1, -2, 2},
        });

        StiffnessTemplate2 = new ImmutableMatrix(new double[,]
        {
            {2, 1, -2, -1},
            {1, 2, -1, -2},
            {-2, -1, 2, 1},
            {-1, -2, 1, 2},
        });

        MassTemplate = new ImmutableMatrix(new double[,]
        {
            {4, 2, 2, 1},
            {2, 4, 1, 2},
            {2, 1, 4, 2},
            {1, 2, 2, 4},
        });
    }

    private readonly HarmonicContext<Point, Element, SparseMatrix> _context;

    public HarmonicLocalAssembler(HarmonicContext<Point, Element, SparseMatrix> context)
    {
        _context = context;
    }

    public void AssembleMatrix(Element element, StackMatrix matrix, StackIndexPermutation indexes)
    {
        var material = _context.Materials.GetById(element.MaterialId);
        var G1 = LinAl.Multiply(
            material.Lambda * element.Width / (element.Length * 6d),
            StiffnessTemplate1
        );
        var G2 = LinAl.Multiply(
            material.Lambda * element.Length / (element.Width * 6d),
            StiffnessTemplate2
        );
        var M = LinAl.Multiply(
            material.Sigma * element.Width * element.Length / 36d,
            MassTemplate
        );

        for (int i = 0, iBlock = 0; i < matrix.Size; i += 2, iBlock++)
        {
            for (int j = 0, jBlock = 0; j < matrix.Size; j += 2, jBlock++)
            {
                matrix[i, j] = G1[iBlock, jBlock] + G2[iBlock, jBlock];
                matrix[i + 1, j + 1] = matrix[i, j];

                matrix[i + 1, j] = _context.Frequency * M[iBlock, jBlock];
                matrix[i, j + 1] = -1d * matrix[i + 1, j];
            }
        }

        FillIndexes(element, indexes);
    }

    public void AssembleRightSide(Element element, Span<double> vector, StackIndexPermutation indexes)
    {
        Span<double> fS = stackalloc double[element.NodeIndexes.Length];
        Span<double> fC = stackalloc double[element.NodeIndexes.Length];
        Span<double> fSTmp = stackalloc double[element.NodeIndexes.Length];
        Span<double> fCTmp = stackalloc double[element.NodeIndexes.Length];

        var defaultMass = LinAl.Multiply(
            element.Width * element.Length / 36d,
            MassTemplate
        );

        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            var f = _context.DensityFunctionProvider.Get(element.NodeIndexes[i]);
            fS[i] = f.Real;
            fC[i] = f.Imaginary;
        }

        var bS = LinAl.Multiply(defaultMass, fS, fSTmp);
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            vector[i * 2] = bS[i];
        }

        var bC = LinAl.Multiply(defaultMass, fC, fCTmp);
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            vector[i * 2 + 1] = bC[i];
        }

        FillIndexes(element, indexes);
    }

    private void FillIndexes(Element element, StackIndexPermutation indexes)
    {
        for (var i = 0; i < element.NodeIndexes.Length; i++)
        {
            indexes.Permutation[i * 2] = element.NodeIndexes[i] * 2;
            indexes.Permutation[i * 2 + 1] = element.NodeIndexes[i] * 2 + 1;
        }
    }
}