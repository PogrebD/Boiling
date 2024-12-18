using SharpMath.Matrices;

namespace SharpMath.FiniteElement.Core.Assembling;

public interface ILocalAssembler<in TElement>
{
    public LocalMatrix AssembleMatrix(TElement element);
    public LocalVector AssembleRightSide(TElement element);
}

public interface IStackLocalAssembler<in TElement>
{
    public void AssembleMatrix(TElement element, StackMatrix matrix, StackIndexPermutation indexes);
    public void AssembleRightSide(TElement element, Span<double> vector, StackIndexPermutation indexes);
}