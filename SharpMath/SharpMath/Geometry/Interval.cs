namespace SharpMath.Geometry;

public readonly struct Interval
{
    public double Begin { get; }
    public double End { get; }

    public double Length => End - Begin;

    public Interval(double begin, double end)
    {
        Begin = begin;
        End = end;
    }

    public bool Has(double value)
    {
        var righter = value >= Begin;
        var lefter = value <= End;
        return righter && lefter;
    }
}
