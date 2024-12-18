using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Second;

public record struct SecondCondition(int ElementId, Bound Bound, double[] Thetta, ComponentType Type);