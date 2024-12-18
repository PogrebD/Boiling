using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling.TemplateMatrices;

public class CylinderTemplateMatrices
{
    public static ImmutableMatrix StiffnessR1D(double r, double elementBoundSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 1d, -1d },
                { -1d, 1d },
            },
            (2 * r + elementBoundSize) / (2 * elementBoundSize)
        );
    }

    public static ImmutableMatrix StiffnessZ1D(double elementBoundSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 1d, -1d },
                { -1d, 1d },
            },
            1 / elementBoundSize
        );
    }

    public static ImmutableMatrix MassR1D(double r, double elementBoundSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                {r * 2d + elementBoundSize / 2, r + elementBoundSize / 2},
                {r + elementBoundSize / 2, r * 2d + elementBoundSize / 2 * 3},
            },
            elementBoundSize / 6
        );
    }

    public static ImmutableMatrix MassZ1D(double elementBoundSize)
    {
        return new ImmutableMatrix
        (
            new[,]
            {
                { 2d, 1d },
                { 1d, 2d },
            },
            elementBoundSize / 6
        );
    }
}