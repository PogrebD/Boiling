using SharpMath.FiniteElement.Core.Assembling;
using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.FiniteElement._2D.Assembling;

public class DenseMatrixInserter : IStackInserter<Matrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector)
    {
        for (var i = 0; i < localVector.IndexPermutation.Length; i++)
        {
            vector[localVector.IndexPermutation.Apply(i)] += localVector[i];
        }
    }

    public void InsertMatrix(Matrix matrix, StackLocalMatrix localMatrix)
    {
        var nodesIndexes = localMatrix.IndexPermutation.Permutation;

        for (var i = 0; i < nodesIndexes.Length; i++)
        {
            for (var j = 0; j < i; j++)
            {
                matrix[nodesIndexes[i], nodesIndexes[j]] += localMatrix[i, j];
                matrix[nodesIndexes[j], nodesIndexes[i]] += localMatrix[j, i];
            }

            matrix[nodesIndexes[i], nodesIndexes[i]] += localMatrix[i, i];
        }
    }
}