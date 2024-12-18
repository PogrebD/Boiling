using SharpMath.Vectors;

namespace SharpMath.EquationsSystem.Preconditions.Diagonal;

public class DiagonalPreconditioner : IPreconditioner
{
    private readonly Vector _inverseDiagonal;

    public DiagonalPreconditioner(Vector diagonal)
    {
        _inverseDiagonal = Vector.Create(diagonal.Length);
        for (var i = 0; i < diagonal.Length; i++)
            _inverseDiagonal[i] = 1d / diagonal[i];
    }

    public DiagonalPreconditioner(IReadOnlyList<double> diagonal)
    {
        _inverseDiagonal = Vector.Create(diagonal.Count);
        for (var i = 0; i < diagonal.Count; i++)
            _inverseDiagonal[i] = 1d / diagonal[i];
    }

    public Vector MultiplyOn(Vector v, Vector? resultMemory = null)
    {
        if (_inverseDiagonal.Length != v.Length)
            throw new ArgumentOutOfRangeException($"{nameof(v)} must have length {_inverseDiagonal.Length}");

        resultMemory ??= new Vector(new double[v.Length]);

        for (int i = 0; i < v.Length; i++)
            resultMemory[i] = _inverseDiagonal[i] * v[i];

        return resultMemory;
    }
}