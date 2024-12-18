using SharpMath.Vectors;

namespace SharpMath.FiniteElement.Core.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertVector(Vector vector, LocalVector localVector);
    public void InsertMatrix(TMatrix matrix, LocalMatrix localMatrix);
}

public interface IStackInserter<in TMatrix>
{
    public void InsertVector(Vector vector, StackLocalVector localVector);
    public void InsertMatrix(TMatrix matrix, StackLocalMatrix localMatrix);
}