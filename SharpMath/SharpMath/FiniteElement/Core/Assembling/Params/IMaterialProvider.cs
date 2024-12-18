namespace SharpMath.FiniteElement.Core.Assembling.Params;

public interface IMaterialProvider<out TMaterial>
{ 
    public TMaterial GetById(int materialId);
}