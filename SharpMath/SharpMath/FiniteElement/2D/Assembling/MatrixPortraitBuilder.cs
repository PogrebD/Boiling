using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Matrices.Sparse;

namespace SharpMath.FiniteElement._2D.Assembling;

// Пишется для гармонической 2D с линейными элементами
public class MatrixPortraitBuilder : IMatrixPortraitBuilder<SparseMatrix, Element>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SparseMatrix Build(IEnumerable<Element> elements, int nodesCount)
    {
        BuildAdjacencyList(elements, nodesCount);

        var amount = 0;
        var buf = _adjacencyList.Select(nodeSet => amount += nodeSet.Count).ToList();
        buf.Insert(0, 0);

        var rowsIndexes = buf.ToArray();
        var columnsIndexes = _adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(IEnumerable<Element> elements, int nodesCount)
    {
        _adjacencyList = new List<SortedSet<int>>(nodesCount * 2);

        for (var i = 0; i < nodesCount * 2; i++)
        {
            _adjacencyList.Add(new SortedSet<int>());
        }

        foreach (var element in elements)
        {
            var nodesIndexes = element.NodeIndexes;

            foreach (var currentNode in nodesIndexes)
            {
                for (var i = 0; i < 2; i++)
                {
                    var currentComplexNode = currentNode * 2 + i;

                    foreach (var nodeIndex in nodesIndexes)
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            var complexNodeIndex = nodeIndex * 2 + j;
                            if (currentComplexNode > complexNodeIndex)
                                _adjacencyList[currentComplexNode].Add(complexNodeIndex);
                        }
                    }
                }
            }
        }
    }
}