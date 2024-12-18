using SharpMath.Vectors;

namespace SharpMath.Matrices.Transformation;

// ReSharper disable once InconsistentNaming
public class LUMatrix
{
    private readonly double[,] _values;

    public LUMatrix(double[,] l, double[,] u)
    {
        throw new NotImplementedException();
    }

    public Vector SolveEqualTo(Vector rightSide, Vector? resultMemory = null)
    {
        if (rightSide.Length != _values.GetLength(0))
            throw new ArgumentOutOfRangeException();
        LinAl.ValidateOrAllocateIfNull(rightSide, ref resultMemory!);

        // TODO удалить вектор y если можно
        // Или сделать его через StackAllocated
        var y = new double[rightSide.Length];

        // Прямой ход
        for (var i = 0; i < rightSide.Length; i++)
        {
            var sum = 0d;
            for (var j = 0; j < i; j++)
            {
                sum += _values[i, j] * y[j];
            }

            y[i] = rightSide[i] - sum;
        }

        // Обратный ход
        for (var i = rightSide.Length - 1; i >= 0; i--)
        {
            var sum = 0d;
            for (var j = rightSide.Length - 1; j > i; j--)
            {
                sum += _values[i, j] * resultMemory[j];
            }

            resultMemory[i] = (y[i] - sum) / _values[i, i];
        }

        return resultMemory;
    }
}