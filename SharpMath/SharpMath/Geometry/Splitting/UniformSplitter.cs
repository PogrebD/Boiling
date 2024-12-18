namespace SharpMath.Geometry.Splitting;

public readonly struct UniformSplitter : IIntervalSplitter
{
    private readonly int _steps;

    public UniformSplitter(int steps)
    {
        _steps = steps;
    }

    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length / _steps;

        var stepNumber = 0;
        var value = interval.Begin + stepNumber * step;

        while (interval.Has(value))
        {
            yield return value;
            stepNumber++;
            value = interval.Begin + stepNumber * step;
        }
    }
}