using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;

namespace SharpMath.FiniteElement.Materials.Providers;

public class FromArrayMaterialProvider : IMaterialProvider<Material>
{
    private readonly Material[] _materials;

    public FromArrayMaterialProvider(Material[] materials)
    {
        _materials = materials;
    }

    public Material GetById(int materialId)
    {
        return _materials[materialId];
    }
}