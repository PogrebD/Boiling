namespace SharpMath.FiniteElement.Core.Harmonic;

public class HarmonicContext<TPoint, TElement, TMatrix> : Context<TPoint, TElement, TMatrix>
{
    public required double Frequency { get; set; }
}