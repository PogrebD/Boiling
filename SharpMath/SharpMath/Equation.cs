using SharpMath.Vectors;

namespace SharpMath;

// TODO replace with IVector<double> Solution and RightSide
public record Equation<TMatrix>(TMatrix Matrix, Vector Solution, Vector RightSide);