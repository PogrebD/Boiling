using SharpMath.Matrices.Sparse;

namespace SharpMath.EquationsSystem.Preconditions;

public class LUPreconditioner : IPreconditioner<SparseMatrix>
{
    public SparseMatrix Decompose(SparseMatrix matrix)
    {
        var preconditionMatrix = matrix.Clone();

        for (var i = 0; i < preconditionMatrix.RowsCount; i++)
        {
            var sumD = 0.0;
            for (var j = preconditionMatrix.RowsIndexes[i]; j < preconditionMatrix.RowsIndexes[i + 1]; j++)
            {
                var sumL = 0d;
                var sumU = 0d;

                for (var k = preconditionMatrix.RowsIndexes[i]; k < j; k++)
                {
                    var iPrev = i - preconditionMatrix.ColumnsIndexes[j];
                    var kPrev = preconditionMatrix[i - iPrev, preconditionMatrix.ColumnsIndexes[k]];

                    if (kPrev == -1) continue;

                    sumL += preconditionMatrix.LowerValues[k] * preconditionMatrix.UpperValues[kPrev];
                    sumU += preconditionMatrix.UpperValues[k] * preconditionMatrix.LowerValues[kPrev];
                }

                preconditionMatrix.LowerValues[j] -= sumL;
                preconditionMatrix.UpperValues[j] = (preconditionMatrix.UpperValues[j] - sumU) / preconditionMatrix.Diagonal[preconditionMatrix.ColumnsIndexes[j]];

                sumD += preconditionMatrix.LowerValues[j] * preconditionMatrix.UpperValues[j];
            }

            preconditionMatrix.Diagonal[i] -= sumD;
        }

        return preconditionMatrix;
    }
}