using SharpMath.Matrices.Sparse;

namespace SharpMath.Matrices.Converters;

public class MatrixConverter
{
    public static ProfileMatrix Convert(SparseMatrix sparseMatrix)
    {
        var diagonal = sparseMatrix.CloneDiagonal();
        var rowsIndexes = sparseMatrix.CloneRows();
        var lowerValues = new List<double>();
        var upperValues = new List<double>();

        for (var i = 1; i < rowsIndexes.Length; i++)
        {
            var rowBegin = i - 1;

            var j = sparseMatrix.RowsIndexes[i - 1];

            for (; j < sparseMatrix.RowsIndexes[i]; j++)
            {
                if (Math.Abs(sparseMatrix.LowerValues[j]) == 0
                    && Math.Abs(sparseMatrix.UpperValues[j]) == 0) continue;
                rowBegin = sparseMatrix.ColumnsIndexes[j];
                break;
            }

            rowsIndexes[i] = rowsIndexes[i - 1] + (i - 1 - rowBegin);

            for (var k = rowsIndexes[i - 1]; k < rowsIndexes[i]; k++, rowBegin++)
            {
                if (sparseMatrix[i - 1, rowBegin] != -1)
                {
                    lowerValues.Add(sparseMatrix.LowerValues[sparseMatrix[i - 1, rowBegin]]);
                    upperValues.Add(sparseMatrix.UpperValues[sparseMatrix[i - 1, rowBegin]]);
                }
                else
                {
                    lowerValues.Add(0d);
                    upperValues.Add(0d);
                }
            }
        }

        return new ProfileMatrix(rowsIndexes, diagonal, lowerValues, upperValues);
    }
}