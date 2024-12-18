using SharpMath.FiniteElement._2D;
using SharpMath.FiniteElement.Materials.MaterialSetter;
using SharpMath.FiniteElement.Materials.MaterialSetter.Factory;
using SharpMath.Geometry.Splitting;

namespace SharpMath.Geometry._2D;

public class GridBuilder
{
    private AxisSplitParameter? _xAxisSplitParameter;
    private AxisSplitParameter? _yAxisSplitParameter;
    private IMaterialSetterFactory _materialSetterFactory = new NullMaterialSetterFactory();

    public GridBuilder SetXAxis(AxisSplitParameter splitParameter)
    {
        _xAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder SetYAxis(AxisSplitParameter splitParameter)
    {
        _yAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder SetMaterialSetterFactory(IMaterialSetterFactory materialSetterFactory)
    {
        _materialSetterFactory = materialSetterFactory;
        return this;
    }

    public Grid<Point, Element> Build()
    {
        var points = CreatePoints();

        var materialSetter = _materialSetterFactory.Create(points);
        var elements = CreateElements(points, materialSetter);

        return new Grid<Point, Element>(
            points,
            elements
        );
    }

    private PointsCollection CreatePoints()
    {
        if (_xAxisSplitParameter == null || _yAxisSplitParameter == null)
            throw new ArgumentNullException();

        var xNodes = _xAxisSplitParameter.CreateAxis().ToArray();
        var yNodes = _yAxisSplitParameter.CreateAxis().ToArray();

        return new PointsCollection(xNodes, yNodes);
    }

    private Element[] CreateElements(PointsCollection nodes, IMaterialSetter materialSetter)
    {
        var totalXElements = (nodes.XLength - 1);
        var totalYElements = (nodes.YLength - 1);
        var totalElements = totalXElements * totalYElements;

        var elements = new Element[totalElements];
        Allocate(elements);

        for (var topRow = 1; topRow < nodes.YLength; topRow++)
        {
            for (var rightColumn = 1; rightColumn < nodes.XLength; rightColumn++)
            {
                var elementIndex = rightColumn - 1 + (topRow - 1) * totalXElements;
                var element = elements[elementIndex];
                FillNodeIndexes(topRow - 1, rightColumn - 1, element.NodeIndexes, totalXElements);

                var indexes = element.NodeIndexes;

                var leftBottom = nodes[indexes[0]];
                var leftTop = nodes[indexes[2]];
                var rightBottom = nodes[indexes[1]];

                element.Length = rightBottom.X - leftBottom.X;
                element.Width = leftTop.Y - leftBottom.Y;

                materialSetter.SetMaterial(element);
            }
        }

        return elements;
    }

    private void FillNodeIndexes(int bottomRow, int leftColumn, int[] indexes, int totalXElements)
    {
        indexes[0] = leftColumn + bottomRow * (totalXElements + 1);
        indexes[1] = leftColumn + 1 + bottomRow * (totalXElements + 1);
        indexes[2] = leftColumn + (bottomRow + 1) * (totalXElements + 1);
        indexes[3] = leftColumn + 1 + (bottomRow + 1) * (totalXElements + 1);
    }

    private void Allocate(IList<Element> elements)
    {
        for (var i = 0; i < elements.Count; i++)
            elements[i] = new Element(new int[4], 0, 0);
    }
}