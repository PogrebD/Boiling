namespace SharpMath.Geometry.Splitting;

public class AxisSplitParameter
{
    public Interval[] Sections { get; }
    public IIntervalSplitter[] Splitters { get; }

    public IEnumerable<(Interval section, IIntervalSplitter parameter)> SectionWithParameter =>
        Sections.Select((section, index) => new ValueTuple<Interval, IIntervalSplitter>(section, Splitters[index]));

    public AxisSplitParameter(double[] points, params IIntervalSplitter[] splitters)
    {
        if (points.Length - 1 != splitters.Length)
            throw new ArgumentException();

        Sections = GenerateSections(points).ToArray();
        Splitters = splitters;
    }

    public IEnumerable<double> CreateAxis()
    {
        foreach (var value in Splitters[0].EnumerateValues(Sections[0]))
        {
            yield return value;
        }

        for (var i = 1; i < Splitters.Length; i++)
        {
            foreach (var value in Splitters[i].EnumerateValues(Sections[i]).Skip(1))
            {
                yield return value;
            }
        }
    }

    private IEnumerable<Interval> GenerateSections(IEnumerable<double> points)
    {
        var left = points.First();
        foreach (var point in points.Skip(1))
        {
            var end = point;
            yield return new Interval(left, end);

            left = end;
        }
    }
}