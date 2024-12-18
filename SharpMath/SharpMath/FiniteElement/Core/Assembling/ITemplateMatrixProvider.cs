using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling;

public interface ITemplateMatrixProvider
{
    public ImmutableMatrix GetMatrix();
}