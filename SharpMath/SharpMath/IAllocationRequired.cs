namespace SharpMath;

public interface IAllocationRequired<in T>
{
    public void Allocate(T param);
}