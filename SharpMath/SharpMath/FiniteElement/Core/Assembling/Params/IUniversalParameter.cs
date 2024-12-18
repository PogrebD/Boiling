namespace SharpMath.FiniteElement.Core.Assembling.Params;

public interface IUniversalParameter<in T, out TResult>
{
    public TResult Get(T parameter);
}