using SharpMath.Matrices;
using SharpMath.Matrices.Sparse;
using SharpMath.Matrices.Transformation;
using SharpMath.Vectors;

namespace SharpMath;

public static class LinAl
{
    // TODO replace Vector with IVector and IReadonlyVector where possible
    public static Vector Sum(Vector v, Vector u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, 1.0, resultMemory);
    }
    public static Vector Subtract(IReadonlyVector<double> v, IReadonlyVector<double> u, Vector? resultMemory = null)
    {
        return LinearCombination(v, u, 1.0, -1.0, resultMemory);
    }
    public static Vector LinearCombination(
        IReadonlyVector<double> v, IReadonlyVector<double> u,
        double vCoefficient, double uCoefficient,
        Vector? resultMemory = null
    )
    {
        AssertSameSize(v, u);
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = v[i] * vCoefficient + u[i] * uCoefficient;

        return resultMemory;
    }

    public static Vector Multiply(double coefficient, Vector v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        for (var i = 0; i < v.Length; i++)
            resultMemory[i] = coefficient * v[i];

        return resultMemory;
    }
    public static Matrix Multiply(double coefficient, MatrixBase a, Matrix? resultMemory = null)
    {
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }
    public static StackMatrix Multiply(double coefficient, MatrixBase a, StackMatrix resultMemory)
    {
        if (a.Rows != resultMemory.Size || a.Columns != resultMemory.Size)
        {
            throw new ArgumentException();
        }

        for (var i = 0; i < a.Rows; i++)
        for (var j = 0; j < a.Columns; j++)
            resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }
    public static StackMatrix Multiply(double coefficient, StackMatrix a, StackMatrix resultMemory)
    {
        if (a.Size != resultMemory.Size)
        {
            throw new ArgumentException();
        }

        for (var i = 0; i < a.Size; i++)
        for (var j = 0; j < a.Size; j++)
            resultMemory[i, j] = a[i, j] * coefficient;

        return resultMemory;
    }
    public static ImmutableMatrix Multiply(double coefficient, ImmutableMatrix a)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
    }
    public static ImmutableMatrix Multiply(ImmutableMatrix a, double coefficient)
    {
        return new ImmutableMatrix(a, a.Coefficient * coefficient);
    }

    public static Vector Multiply(SparseMatrix a, Vector v, Vector? resultMemory = null)
    {
        if (resultMemory is null)
        {
            resultMemory = Vector.Create(a.RowsCount);
        } 
        else if (resultMemory.Length != a.RowsCount)
        {
            throw new ArgumentException();
        }

        var rowsIndexes = a.RowsIndexes;
        var columnsIndexes = a.ColumnsIndexes;
        var di = a.Diagonal;
        var lowerValues = a.LowerValues;
        var upperValues = a.UpperValues;

        for (var i = 0; i < a.RowsCount; i++)
        {
            resultMemory[i] += di[i] * v[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                resultMemory[i] += lowerValues[j] * v[columnsIndexes[j]];
                resultMemory[columnsIndexes[j]] += upperValues[j] * v[i];
            }
        }

        return resultMemory;
    }
    public static Vector Multiply(MatrixBase a, IReadonlyVector<double> v, Vector? resultMemory = null)
    {
        ValidateOrAllocateIfNull(v, ref resultMemory!);

        //var result = new double[v.TotalPoints];

        for (var i = 0; i < v.Length; i++)
            for (var j = 0; j < v.Length; j++)
                resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    public static Span<double> Multiply(StackMatrix a, ReadOnlySpan<double> v, Span<double> resultMemory)
    {
        if (a.Size != v.Length || v.Length != resultMemory.Length)
        {
            throw new ArgumentException();
        }
        
        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }
    public static Span<double> Multiply(MatrixBase a, ReadOnlySpan<double> v, Span<double> resultMemory)
    {
        if (a.Columns != v.Length || a.Rows != v.Length || v.Length != resultMemory.Length)
        {
            throw new ArgumentException();
        }

        for (var i = 0; i < v.Length; i++)
        for (var j = 0; j < v.Length; j++)
            resultMemory[i] += a[i, j] * v[j];

        return resultMemory;
    }

    public static Vector Multiply(SymmetricSparseMatrix matrix, Vector x, Vector? resultMemory = null)
    {
        if (resultMemory == null)
        {
            resultMemory = new Vector(new double[x.Length]);
        }
        else
        {
            AssertSameSize(x, resultMemory);
        }
        if (matrix.RowIndexes.Length != x.Length)
        {
            throw new ArgumentOutOfRangeException($"{nameof(matrix.RowIndexes)} and {nameof(x)}", "must have the same length");
        }

        for (var i = 0; i < x.Length; i++)
            resultMemory[i] = x[i] * matrix.Diagonal[i];

        for (var i = 0; i < x.Length; i++)
        {
            foreach (var indexValue in matrix[i])
            {
                resultMemory[i] += indexValue.Value * x[indexValue.ColumnIndex];
                resultMemory[indexValue.ColumnIndex] += indexValue.Value * x[i];
            }
        }

        return resultMemory;
    }
    public static Matrix Multiply(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertSameSize(a, b);
        if (a == resultMemory || b == resultMemory) 
            throw new ArgumentOutOfRangeException($"{nameof(resultMemory)}", "can't be equal to one of the arguments");
        resultMemory = ValidateOrAllocateIfNullForMultiplying(a, b, resultMemory).AsMutable();

        for (var aRow = 0; aRow < a.Rows; aRow++)
        {
            for (var bColumn = 0; bColumn < b.Columns; bColumn++)
            {
                double sum = 0;
                for (var i = 0; i < a.Columns; i++)
                    sum += a[aRow, i] * b[i, bColumn];

                resultMemory[aRow, bColumn] = sum;
            }
        }

        return resultMemory;
    }
    public static SparseMatrix Multiply(double coefficient, SparseMatrix a, SparseMatrix? resultMemory = null)
    {
        resultMemory ??= new SparseMatrix(a.RowsIndexes, a.ColumnsIndexes);

        for (var i = 0; i < a.RowsCount; i++)
        {
            resultMemory.Diagonal[i] = coefficient * a.Diagonal[i];

            for (var j = resultMemory.RowsIndexes[i]; j < resultMemory.RowsIndexes[i + 1]; j++)
            {
                resultMemory.LowerValues[j] = coefficient * a.LowerValues[j];
                resultMemory.UpperValues[j] = coefficient * a.UpperValues[j];
            }
        }

        return resultMemory;
    }
    public static MatrixBase Sum(MatrixBase a, MatrixBase b, Matrix? resultMemory = null)
    {
        AssertCanBeMultiplied(a, b);
        resultMemory = ValidateOrAllocateIfNull(a, resultMemory).AsMutable();

        for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Rows; j++)
                resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    public static StackMatrix Sum(MatrixBase a, MatrixBase b, StackMatrix resultMemory)
    {
        AssertCanBeMultiplied(a, b);

        for (var i = 0; i < a.Rows; i++)
        for (var j = 0; j < a.Rows; j++)
            resultMemory[i, j] = a[i, j] + b[i, j];

        return resultMemory;
    }
    public static SparseMatrix Sum(SparseMatrix a, SparseMatrix b, SparseMatrix? resultMemory = null)
    {
        if (a.RowsCount != b.RowsCount)
        {
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same size");
        }

        resultMemory ??= new SparseMatrix(a.RowsIndexes, a.ColumnsIndexes);

        for (var i = 0; i < a.RowsCount; i++)
        {
            resultMemory.Diagonal[i] = a.Diagonal[i] + b.Diagonal[i];

            for (var j = resultMemory.RowsIndexes[i]; j < resultMemory.RowsIndexes[i + 1]; j++)
            {
                resultMemory.LowerValues[j] = a.LowerValues[j] + b.LowerValues[j];
                resultMemory.UpperValues[j] = a.UpperValues[j] + b.UpperValues[j];
            }
        }

        return resultMemory;
    }


    private static MatrixBase ValidateOrAllocateIfNull(MatrixBase a, MatrixBase? b)
    {
        if (b is null)
            b = new Matrix(new double[a.Rows, a.Columns]);
        else AssertSameSize(a, b);

        return b;
    }
    public static void AssertSameSize(MatrixBase a, MatrixBase b)
    {
        if (a.Rows != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same rows");
        if (a.Columns != b.Columns)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "must have the same columns");
    }

    private static MatrixBase ValidateOrAllocateIfNullForMultiplying(MatrixBase a, MatrixBase b, MatrixBase? c)
    {
        AssertCanBeMultiplied(a, b);
        if (c is null)
            c = new Matrix(new double[a.Rows, b.Columns]);
        else if (a.Rows != c.Rows || b.Columns != c.Columns)
            throw new ArgumentOutOfRangeException(nameof(c), "сan't be the result of a multiplication");

        return c;
    }
    public static void AssertCanBeMultiplied(MatrixBase a, MatrixBase b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentOutOfRangeException($"{nameof(a)} and {nameof(b)}", "can't be multiplied");
    }
    
    public static void ValidateOrAllocateIfNull(IReadonlyVector<double> v, ref Vector? u)
    {
        if (u is null)
            u = Vector.Create(v.Length);
        else AssertSameSize(v, u);
    }
    private static void AssertSameSize<T>(IReadonlyVector<T> v, IReadonlyVector<T> u)
    {
        if (v.Length != u.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} and {nameof(u)}", "must have the same length");
    }
}