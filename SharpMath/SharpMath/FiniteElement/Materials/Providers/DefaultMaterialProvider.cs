using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;

namespace SharpMath.FiniteElement.Materials.Providers;

public class DefaultMaterialProvider : IMaterialProvider<Material>
{
    private static readonly Material Material = new (1d, 1d);

    public Material GetById(int materialId) => Material;
}