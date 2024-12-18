using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.Boiling;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;

namespace SharpMath.FiniteElement.Materials.Providers;

public class BoilingMaterialProvider : IMaterialProvider<BoilingMaterial>
{
    private readonly BoilingMaterial[] _materials;

    public BoilingMaterialProvider(BoilingMaterial[] materials)
    {
        _materials = materials;
    }

    public BoilingMaterial GetById(int materialId)
    {
        return _materials[materialId];
    }
}