namespace SharpMath.FiniteElement.Core.Assembling;

public interface IMatrixPortraitBuilder<out TMatrix, in TElement>
{
    TMatrix Build(IEnumerable<TElement> elements, int nodesCount);
}