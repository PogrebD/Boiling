using SharpMath.FiniteElement.Core.Assembling.Boundary.First;
using SharpMath.FiniteElement.Core.Assembling.Boundary.Second;
using SharpMath.FiniteElement.Core.Assembling.Params;
using SharpMath.FiniteElement.Materials.HarmonicWithoutChi;
using SharpMath.Geometry;
using System.Numerics;

namespace SharpMath.Splines;

public class SplineContext<TPoint, TElement, TMatrix>
{
    public required Grid<TPoint, TElement> Grid { get; set; }
    public required Equation<TMatrix> Equation { get; set; }
    public required FuncValue[] FunctionValues { get; set; }
    public required double[] Weights { get; set; }
    public required double Alpha { get; set; }
    public required double Beta { get; set; }
}