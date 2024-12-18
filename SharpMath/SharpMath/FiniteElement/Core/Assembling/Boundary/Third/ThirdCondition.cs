using SharpMath.FiniteElement.Core.Harmonic;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement.Core.Assembling.Boundary.Third;

public record struct ThirdCondition(int ElementId, Bound Bound, double[] UBeta, double Beta);