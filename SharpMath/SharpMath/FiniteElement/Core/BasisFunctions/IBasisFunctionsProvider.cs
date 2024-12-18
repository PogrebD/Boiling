using SharpMath.FiniteElement.Materials;

namespace SharpMath.FiniteElement.Core.BasisFunctions;

public interface IBasisFunctionsProvider<in TElement, in TPoint>
{
    public IBasisFunction<TPoint>[] GetFunctions(TElement element);
}