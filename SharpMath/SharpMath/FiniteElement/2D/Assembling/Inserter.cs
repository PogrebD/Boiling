using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.FiniteElement._2D.Assembling;

public class Inserter : IStackInserter<SparseMatrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector)
    {
        for (var i = 0; i < localVector.IndexPermutation.Length; i++)
        {
            vector[localVector.IndexPermutation.Apply(i)] += localVector[i];
        }
    }

    public void InsertMatrix(SparseMatrix matrix, StackLocalMatrix localMatrix)
    {
        var nodesIndexes = localMatrix.IndexPermutation.Permutation;

        for (var i = 0; i < nodesIndexes.Length; i++)
        {
            for (var j = 0; j < i; j++)
            {
                var elementIndex = matrix[nodesIndexes[i], nodesIndexes[j]];

                if (elementIndex == -1) continue;
                matrix.LowerValues[elementIndex] += localMatrix[i, j];
                matrix.UpperValues[elementIndex] += localMatrix[j, i];
            }

            matrix.Diagonal[nodesIndexes[i]] += localMatrix[i, i];
        }
    }
}