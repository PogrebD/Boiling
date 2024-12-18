namespace SharpMath.FiniteElement.Materials.MaterialSetter.Areas;

public interface IMaterialArea<in TPoint>
{
    public int MaterialId { get; }
    public bool Contains(TPoint point);
}