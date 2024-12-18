namespace SharpMath.FiniteElement.Materials;

public interface IFiniteElement
{
    public int MaterialId { get; set; }
    public int[] NodeIndexes { get; set; }
}