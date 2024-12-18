namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Third;

public interface IThirdBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> equation, ThirdCondition condition);
}