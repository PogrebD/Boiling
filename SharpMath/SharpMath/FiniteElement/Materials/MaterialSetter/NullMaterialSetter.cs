namespace SharpMath.FiniteElement.Materials.MaterialSetter;

public class NullMaterialSetter : IMaterialSetter
{
    public void SetMaterial(IFiniteElement element)
    {
        element.MaterialId = 0;
    }
}