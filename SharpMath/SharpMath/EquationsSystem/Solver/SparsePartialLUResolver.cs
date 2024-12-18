using SharpMath.Matrices.Sparse;
using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Solver;

//Todo придумать название для методов более ясное
public class SparsePartialLUResolver
{
    public Vector CalcY(SparseMatrix sparseMatrix, Vector b)
    {
        var y = b;

        for (var i = 0; i < sparseMatrix.RowsCount; i++)
        {
            var sum = 0.0;
            for (var j = sparseMatrix.RowsIndexes[i]; j < sparseMatrix.RowsIndexes[i + 1]; j++)
            {
                sum += sparseMatrix.LowerValues[j] * y[sparseMatrix.ColumnsIndexes[j]];
            }
            y[i] = (b[i] - sum) / sparseMatrix.Diagonal[i];
        }
        return y;
    }

    public Vector CalcX(SparseMatrix sparseMatrix, Vector y)
    {
        var x = y.CopyTo(new double[sparseMatrix.RowsCount]);

        for (var i = sparseMatrix.RowsCount - 1; i >= 0; i--)
        {
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.UpperValues[j] * x[i];
            }
        }

        return x;
    }

    //Todo доработать если возможно и заменить метод CalcX на этот
    public void CalcXWithoutMemory(SparseMatrix sparseMatrix, Vector y)
    {
        var x = y;

        for (var i = sparseMatrix.RowsCount - 1; i >= 0; i--)
        {
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.UpperValues[j] * x[i];
            }
        }
    }
}