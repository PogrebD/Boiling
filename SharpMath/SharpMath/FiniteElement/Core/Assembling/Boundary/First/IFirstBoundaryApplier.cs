namespace SharpMath.FiniteElement.Core.Assembling.Boundary.First;

public interface IFirstBoundaryApplier<TMatrix>
{
    public void Apply(Equation<TMatrix> matrix, FirstCondition condition);
}