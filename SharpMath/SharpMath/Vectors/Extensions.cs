namespace SharpMath.Vectors;

public static class Extensions
{
    public static bool IsAnyNaN(this IReadonlyVector<double> vector)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            if (double.IsNaN(vector[i]))
            {
                return true;
            }
        }

        return false;
    }

    public static void Nullify(this IVector<double> vector)
    {
        for (var i = 0; i < vector.Length; i++)
        {
            vector[i] = 0;
        }
    }
}