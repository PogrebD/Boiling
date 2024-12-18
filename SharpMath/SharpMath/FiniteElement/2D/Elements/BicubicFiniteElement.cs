using SharpMath.FiniteElement.Materials;
using SharpMath.Geometry._2D;

namespace SharpMath.FiniteElement._2D.Elements;

public class BicubicFiniteElement : IFiniteElement
{
    public BicubicFiniteElement(int[] NodeIndexes, double Length, double Width, int MaterialId = 0)
    {
        this.NodeIndexes = NodeIndexes;
        this.Length = Length;
        this.Width = Width;
        this.MaterialId = MaterialId;
    }

    public const int StepsInsideElement = 3;
    public const int NodesOnBound = StepsInsideElement + 1;
    public const int NodesInElement = NodesOnBound * NodesOnBound;
    public int[] NodeIndexes { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public int MaterialId { get; set; }

    public int[] GetBoundNodeIndexes(Bound bound) =>
        bound switch
        {
            Bound.Left => new[]
            {
                NodeIndexes[0],
                NodeIndexes[4],
                NodeIndexes[8],
                NodeIndexes[12],
            },
            Bound.Right => new[]
            {
                NodeIndexes[3],
                NodeIndexes[7],
                NodeIndexes[11],
                NodeIndexes[15],
            },
            Bound.Bottom => new[]
            {
                NodeIndexes[0],
                NodeIndexes[1],
                NodeIndexes[2],
                NodeIndexes[3],
            },
            Bound.Top => new[]
            {
                NodeIndexes[12],
                NodeIndexes[13],
                NodeIndexes[14],
                NodeIndexes[15],
            },
            _ => throw new ArgumentOutOfRangeException()
        };

    public Span<int> GetBoundNodeIndexes(Bound bound, Span<int> resultMemory)
    {
        switch(bound)
        {
            case Bound.Left:
                resultMemory[0] = NodeIndexes[0];
                resultMemory[1] = NodeIndexes[4];
                resultMemory[2] = NodeIndexes[8];
                resultMemory[3] = NodeIndexes[12];
            break;
            case Bound.Right: 
                resultMemory[0] = NodeIndexes[3];
                resultMemory[1] = NodeIndexes[7];
                resultMemory[2] = NodeIndexes[11];
                resultMemory[3] = NodeIndexes[15];
            break;
            case Bound.Bottom:
                resultMemory[0] = NodeIndexes[0];
                resultMemory[1] = NodeIndexes[1];
                resultMemory[2] = NodeIndexes[2];
                resultMemory[3] = NodeIndexes[3];
                break;
            case Bound.Top:
                resultMemory[0] = NodeIndexes[12];
                resultMemory[1] = NodeIndexes[13];
                resultMemory[2] = NodeIndexes[14];
                resultMemory[3] = NodeIndexes[15];
                break;
        };
        return resultMemory;
    }

    public void Deconstruct(out int[] NodeIndexes, out double Length, out double Width, out int MaterialId)
    {
        NodeIndexes = this.NodeIndexes;
        Length = this.Length;
        Width = this.Width;
        MaterialId = this.MaterialId;
    }
}