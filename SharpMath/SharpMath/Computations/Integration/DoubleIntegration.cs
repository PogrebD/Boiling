using SharpMath.Geometry;

namespace SharpMath.Computations.Integration;

public class DoubleIntegration
{
    private readonly GaussMethodConfig _config;

    public DoubleIntegration(GaussMethodConfig config)
    {
        _config = config;
    }

    public double Integrate(Interval rInterval, Interval zInterval, Func<double, double, double> function)
    {
        var rStep = rInterval.Length / _config.NumberOfSegments;
        var zStep = zInterval.Length / _config.NumberOfSegments;

        var outerIntegralValue = 0.0;

        for (var i = 0; i < _config.GaussMethodNumber; i++)
        {
            var sumOfOuterIntegral = 0.0;

            for (var r = 0; r < _config.NumberOfSegments; r++)
            {
                var rI = (rInterval.Begin + r * rStep + rInterval.Begin + (r + 1) * rStep) / 2.0 + _config.InterpolationNodes[i] * rStep / 2.0;
                var innerIntegralValue = 0.0;

                for (var j = 0; j < _config.GaussMethodNumber; j++)
                {
                    var sumOfInnerIntegral = 0.0;
                    for (var z = 0; z < _config.NumberOfSegments; z++)
                    {
                        var zJ = (zInterval.Begin + z * zStep + zInterval.Begin + (z + 1) * zStep) / 2.0 + _config.InterpolationNodes[j] * zStep / 2.0;
                        sumOfInnerIntegral += zStep * function(rI, zJ);
                    }

                    innerIntegralValue += sumOfInnerIntegral * _config.Weights[j] / 2.0;
                }

                sumOfOuterIntegral += rStep * innerIntegralValue;
            }
            outerIntegralValue += _config.Weights[i] / 2.0 * sumOfOuterIntegral;
        }

        return outerIntegralValue;
    }
}

public class GaussMethodConfig
{
    public double[] InterpolationNodes { get; }

    public double[] Weights { get; }

    public int GaussMethodNumber { get; }

    public int NumberOfSegments { get; }

    private GaussMethodConfig(double[] interpolationNodes, double[] weights, int gaussMethodNumber, int numberOfSegments)
    {
        InterpolationNodes = interpolationNodes;
        Weights = weights;
        GaussMethodNumber = gaussMethodNumber;
        NumberOfSegments = numberOfSegments;
    }

    public static GaussMethodConfig UseGaussMethodTwo(int numberOfSegments)
    {
        return new GaussMethodConfig(
            new[] { -0.5773503, 0.5773503 }, 
            new[] { 1.0, 1.0 }, 
            2,
            numberOfSegments
        );
    }
}